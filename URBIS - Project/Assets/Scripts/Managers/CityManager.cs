using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CityManager : MonoBehaviour
{
    private static CityManager instance;

    public static CityManager Instance => instance;

    [Tooltip("Mark the object as DontDestroyOnLoad.")]
    [SerializeField] private bool isPersistent;
    [Tooltip("Money to be received at the start of the game.")]
    [SerializeField] private float startingMoney = 500;
    [Tooltip("Distance between houses, to be used when spawning houses so they stay in a grid-like pattern.")]
    [SerializeField] private Vector3 distanceBetweenHouses = new Vector3(10, 0, 10);
    [Tooltip("Time between spawning a random hospital.")]
    [SerializeField] private float spawnHouseCooldown = 5.0f;

    [Space, Header("Losing condition attributes")]
    
    [Tooltip("If the average happiness is lower than this for too long, the game will end.")]
    [SerializeField] private float minHappiness = 0.8f;
    [Tooltip("If the average carbon emission is greater than this for too long, the game will end.")]
    [SerializeField] private float maxCarbonEmission = 0.8f;
    [Tooltip("If the average tax income is less than this, the game will end.")]
    [SerializeField] private float minTaxIncomePerHouse = -2.0f;
    
    [Space]
    
    [Tooltip("Time in seconds the player can stay with low happiness before losing.")]
    [SerializeField] private float lowHappinessDuration = 180;
    [Tooltip("Time in seconds the player can stay with low tax income before losing.")]
    [SerializeField] private float lowTaxIncomeDuration = 180;
    [Tooltip("Time in seconds the player can stay with high carbon emission before losing.")]
    [SerializeField] private float highCarbonEmissionDuration = 180;

    [Space]
    [SerializeField] private Color losingColor = new Color(1.0f, 0.5f, 0.5f);
    [SerializeField] private Color regularColor = new Color(1.0f, 1.0f, 1.0f);
    
    
    [Space, Header("Prefabs")]
    
    [Tooltip("Object that will be spawned at empty tiles.")]
    [SerializeField] private GameObject emptyTileHighlight;

    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject hospitalPrefab;
    [SerializeField] private GameObject policeDepartmentPrefab;
    [SerializeField] private GameObject busStopPrefab;

    [Space, Header("UI objects")]
    
    [SerializeField] private TextMeshProUGUI guiCurrentMoney;
    [SerializeField] private TextMeshProUGUI guiTaxIncomePerSecond;
    [SerializeField] private TextMeshProUGUI guiAverageHappiness;
    [SerializeField] private TextMeshProUGUI guiAverageCarbonEmission;

    [Space]
    [SerializeField] private GameObject structureInfoWindow;
    [SerializeField] private TextMeshProUGUI structureInfoText;

    [Space]
    [SerializeField] private GameObject gameOverWindow;

    [Header("The below variables are serialized only for being easily viewable on the editor.")]
    
    [SerializeField] private float currentMoney;
    [SerializeField] private float averageCarbonEmission;
    [SerializeField] private float taxIncomePerSecond;
    [SerializeField] private float averageHappiness;
    [SerializeField] private float totalCarbonEmission;
    [SerializeField] private BuildingHouseState currentBuildingState = BuildingHouseState.NotBuilding;
    [SerializeField] private float currentHouseSpawnCooldown = 0.0f;

    private HashSet<House> houses = new HashSet<House>();
    private HashSet<EmptyTile> emptyTiles = new HashSet<EmptyTile>();
    private BaseBuilding selectedObject;

    private IEnumerator lowTaxIncomeCoroutine, lowHappinessCoroutine, highCarbonEmissionCoroutine;

    private EmptyTile[] emptyTilesArray;

    public BaseBuilding SelectedObject
    {
        get => selectedObject;
        //set => selectedObject = value;
    }
    public Vector3 DistanceBetweenHouses
    {
        get => distanceBetweenHouses;
        set => distanceBetweenHouses = value;
    }

    private void Awake()
    {
        currentMoney = startingMoney;
        if (!instance)
        {
            instance = this;
            if(isPersistent)
                DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        CameraMovement.OnMouseClick += MouseClick;
    }

    private void MouseClick(RaycastHit hitobject)
    {
        if (hitobject.collider == null)
        {
            return;
        }
        if (hitobject.transform.TryGetComponent(out EmptyTile emptyTile))
        {
            if (currentBuildingState != BuildingHouseState.NotBuilding)
            {
                BuildSelectedStructure(emptyTile);
            }
        }
    }

    private void Update()
    {
        taxIncomePerSecond = 0.0f;
        averageHappiness = 0.0f;
        totalCarbonEmission = 0.0f;
        
        foreach (House house in houses)
        {
            if (!house)
            {
                houses.Remove(house);
                continue;
            }
            taxIncomePerSecond += house.TaxIncome * house.CurrentHappiness;
            averageHappiness += house.CurrentHappiness;
            totalCarbonEmission += house.CarbonEmission / (house.AmountOfBusStops + 1);
        }

        averageHappiness /= houses.Count;
        averageCarbonEmission = totalCarbonEmission / houses.Count;
        currentMoney += taxIncomePerSecond * Time.deltaTime;

        ProcessLoseConditions();
        
        guiCurrentMoney.SetText($"Fundo reserva: {currentMoney.ToString("0.00")}");
        guiAverageHappiness.SetText($"Felicidade: {averageHappiness.ToString("0.0")}");
        guiAverageCarbonEmission.SetText($"Emissão de CO2: {averageCarbonEmission.ToString("0.0")}");
        guiTaxIncomePerSecond.SetText($"Renda: {averageCarbonEmission.ToString("0.00")}");

        currentHouseSpawnCooldown += Time.deltaTime;
        if (currentHouseSpawnCooldown >= spawnHouseCooldown)
        {
            SpawnRandomHouse();
            currentHouseSpawnCooldown = 0.0f;
        }
    }

    private void ProcessLoseConditions()
    {
        if (averageHappiness < minHappiness && lowHappinessCoroutine == null)
        {
            guiAverageHappiness.color = losingColor;
            lowHappinessCoroutine = LowHappinessLoss();
            StartCoroutine(lowHappinessCoroutine);
        }
        else if (averageHappiness >= minHappiness  && lowHappinessCoroutine != null)
        {
            guiAverageHappiness.color = regularColor;
            StopCoroutine(lowHappinessCoroutine);
            lowHappinessCoroutine = null;
        }
        
        if (averageCarbonEmission > maxCarbonEmission && highCarbonEmissionCoroutine == null)
        {
            guiAverageCarbonEmission.color = losingColor;
            highCarbonEmissionCoroutine = HighCarbonEmissionLoss();
            StartCoroutine(highCarbonEmissionCoroutine);
        }
        else if (averageCarbonEmission <= maxCarbonEmission && highCarbonEmissionCoroutine != null)
        {
            guiAverageCarbonEmission.color = regularColor;
            StopCoroutine(highCarbonEmissionCoroutine);
            highCarbonEmissionCoroutine = null;
        }
        
        if ((taxIncomePerSecond / houses.Count) < minTaxIncomePerHouse && lowTaxIncomeCoroutine == null)
        {
            guiTaxIncomePerSecond.color = losingColor;
            lowTaxIncomeCoroutine = LowTaxIncomeLoss();
            StartCoroutine(lowTaxIncomeCoroutine);
        }
        else if ((taxIncomePerSecond / houses.Count) >= minTaxIncomePerHouse && lowTaxIncomeCoroutine != null)
        {
            guiTaxIncomePerSecond.color = regularColor;
            StopCoroutine(lowTaxIncomeCoroutine);
            lowTaxIncomeCoroutine = null;
        }
            
    }
    
    private void LoseGame()
    {
        Time.timeScale = 0.0f;
        gameOverWindow.SetActive(true);
    }

    private IEnumerator LowHappinessLoss()
    {
        yield return new WaitForSeconds(lowHappinessDuration);
        Debug.Log("<color=red>You lost the game!</color> The citizens were not happy enough.");

        LoseGame();
    }

    private IEnumerator LowTaxIncomeLoss()
    {
        yield return new WaitForSeconds(lowTaxIncomeDuration);
        Debug.Log("<color=red>You lost the game!</color> The tax income was too low.");
        
        LoseGame();
    }
    
    private IEnumerator HighCarbonEmissionLoss()
    {
        yield return new WaitForSeconds(highCarbonEmissionDuration);
        Debug.Log("<color=red>You lost the game!</color> The city's air was too polluted.");
        
        LoseGame();
    }

    private void SpawnRandomHouse()
    {
        var newHouse = Instantiate(housePrefab, transform.position, Quaternion.identity);
        emptyTilesArray = emptyTiles.ToArray();
        var randomTile = emptyTilesArray[Random.Range(0, emptyTilesArray.Length)];
        newHouse.transform.position = randomTile.transform.position;

        emptyTiles.Remove(randomTile);
        Destroy(randomTile.gameObject);
    }
    
    private void BeforeShowInfo()
    {
        if (selectedObject)
        {
            selectedObject.Deselect();
        }
    }

    public void HideInfoWindow()
    {
        selectedObject = null;
    }

    public void ShowHouseInfo(House house)
    {
        BeforeShowInfo();
        selectedObject = house;
        structureInfoWindow.SetActive(true);
        string temp = "Casa\n" +
                      "Renda: " + house.TaxIncome + "\n" +
                      "Felicidade: " + house.CurrentHappiness + "\n" +
                      "Emissão de CO2: " + house.CarbonEmission + "\n";
        if (!house.HasHospital)
        {
            temp += "<color=red>Não há acesso a um hospital!</color>\n";
        }
        if (!house.HasPoliceDepartment)
        {
            temp += "<color=red>Não há acesso a uma delegacia!</color>!";
        }
        structureInfoText.SetText(temp);
    }

    public void ShowHospitalInfo(Hospital hospital)
    {
        selectedObject = hospital;
        structureInfoWindow.SetActive(true);
        string temp = "Hospital\n" +
                      "Casas conectadas: " + hospital.ConnectedHouses;

        structureInfoText.SetText(temp);
    }

    public void ShowBusStopInfo(BusStop busStop)
    {
        selectedObject = busStop;
        structureInfoWindow.SetActive(true);
        string temp = "Parada de ônibus\n" +
                      "Casas conectadas: " + busStop.ConnectedHouses;

        structureInfoText.SetText(temp);
    }
    
    public void ShowPoliceDepartmentInfo(PoliceDepartment policeDepartment)
    {
        selectedObject = policeDepartment;
        structureInfoWindow.SetActive(true);
        string temp = "Delegacia\n" +
                      "Casas conectadas: " + policeDepartment.ConnectedHouses;

        structureInfoText.SetText(temp);
    }

    public void TrackHouse(House house)
    {
        houses.Add(house);
    }

    public void TrackEmptyTile(Vector3 tile)
    {
        GameObject spawnedTile = Instantiate(emptyTileHighlight, tile, Quaternion.identity);
        spawnedTile.transform.SetParent(transform);
        if (currentBuildingState == BuildingHouseState.NotBuilding)
        {
            spawnedTile.SetActive(false);
        }

        emptyTiles.Add(spawnedTile.GetComponentInChildren<EmptyTile>());
    }

    public void StartBuildingHospital()
    {
        switch (currentBuildingState)
        {
            case BuildingHouseState.NotBuilding:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Enable();
                }
                break;
            }
            case BuildingHouseState.BuildingHospital:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Disable();
                }

                currentBuildingState = BuildingHouseState.NotBuilding;
                return;
            }
        }
        
        currentBuildingState = BuildingHouseState.BuildingHospital;
    }
    
    public void StartBuildingPoliceDepartment()
    {
        switch (currentBuildingState)
        {
            case BuildingHouseState.NotBuilding:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Enable();
                }
                break;
            }
            case BuildingHouseState.BuildingPoliceDepartment:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Disable();
                }

                currentBuildingState = BuildingHouseState.NotBuilding;
                return;
            }
        }
        
        currentBuildingState = BuildingHouseState.BuildingPoliceDepartment;
    }
    
    public void StartBuildingBusStop()
    {
        switch (currentBuildingState)
        {
            case BuildingHouseState.NotBuilding:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Enable();
                }
                break;
            }
            case BuildingHouseState.BuildingBusStop:
            {
                foreach (EmptyTile tile in emptyTiles)
                {
                    tile.Disable();
                }

                currentBuildingState = BuildingHouseState.NotBuilding;
                return;
            }
        }
        
        currentBuildingState = BuildingHouseState.BuildingBusStop;
    }

    public void BuildSelectedStructure(EmptyTile tile)
    {
        float constructionCost;
        switch (currentBuildingState)
        {
            case BuildingHouseState.BuildingHospital:
                constructionCost = hospitalPrefab.GetComponent<BaseBuilding>().ConstructionCost;
                if (currentMoney >= constructionCost)
                {
                    Instantiate(hospitalPrefab, tile.transform.position, Quaternion.identity);
                    emptyTiles.Remove(tile);
                    Destroy(tile.gameObject);
                    currentMoney -= constructionCost;
                }
                break;
            case BuildingHouseState.BuildingPoliceDepartment:
                constructionCost = policeDepartmentPrefab.GetComponent<BaseBuilding>().ConstructionCost;
                if (currentMoney >= constructionCost)
                {
                    Instantiate(policeDepartmentPrefab, tile.transform.position, Quaternion.identity);
                    emptyTiles.Remove(tile);
                    Destroy(tile.gameObject);
                    currentMoney -= constructionCost;
                }

                break;
            case BuildingHouseState.BuildingBusStop:
                constructionCost = busStopPrefab.GetComponent<BaseBuilding>().ConstructionCost;
                if (currentMoney >= constructionCost)
                {
                    Instantiate(busStopPrefab, tile.transform.position, Quaternion.identity);
                    emptyTiles.Remove(tile);
                    Destroy(tile.gameObject);
                    currentMoney -= constructionCost;
                }

                break;
            default:
                //Debug.Log("Hello");
                return;
        }
        
        /*currentBuildingState
        foreach (EmptyTile _tile in emptyTiles)
        {
            _tile.Disable();
        }*/
    }
}