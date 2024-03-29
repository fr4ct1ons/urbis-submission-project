﻿using System;
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
    [Tooltip("Color of the text when a certain resource may cause a game loss.")]
    [SerializeField] private Color losingColor = new Color(1.0f, 0.5f, 0.5f);
    [Tooltip("Color of the text when a certain resource is normal.")]
    [SerializeField] private Color regularColor = new Color(1.0f, 1.0f, 1.0f);
    
    
    [Space, Header("Prefabs")]
    
    [Tooltip("Object that will be spawned at empty tiles.")]
    [SerializeField] private GameObject emptyTileHighlight;

    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject hospitalPrefab;
    [SerializeField] private GameObject policeDepartmentPrefab;
    [SerializeField] private GameObject busStopPrefab;

    [Space, Header("Objects on scene")]
    
    [SerializeField] private TextMeshProUGUI guiCurrentMoney;
    [SerializeField] private DayNightCycle dayNightCycle;
    [SerializeField] private TextMeshProUGUI guiTaxIncomePerSecond;
    [SerializeField] private TextMeshProUGUI guiAverageHappiness;
    [SerializeField] private TextMeshProUGUI guiAverageCarbonEmission;
    
    [Header("Values serialized for being easily edited")]
    
    [SerializeField] private float currentMoney;
    
    [SerializeField] private float averageCarbonEmission;
    [SerializeField] private float taxIncomePerSecond;
    [SerializeField] private float averageHappiness;
    [SerializeField] private float totalCarbonEmission;
    [SerializeField] private BuildingHouseState currentBuildingState = BuildingHouseState.NotBuilding;
    [SerializeField] private float currentHouseSpawnCooldown = 0.0f;

    [SerializeField] private List<House> houses = new List<House>();
    [SerializeField] private List<ICostlyBuilding> costlyBuildings = new List<ICostlyBuilding>();
    private HashSet<EmptyTile> emptyTiles = new HashSet<EmptyTile>();

    private IEnumerator lowTaxIncomeCoroutine, lowHappinessCoroutine, highCarbonEmissionCoroutine;

    private EmptyTile[] emptyTilesArray;
    
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

    /// <summary>
    /// Builds the selected structure, if the player clicks an empty tile.
    /// </summary>
    /// <param name="hitobject"></param>
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

    /// <summary>
    /// Gets the city's current tax income, setups average carbon emission and happiness and updates the UI.
    /// </summary>
    private void Update()
    {
        taxIncomePerSecond = 0.0f;
        averageHappiness = 0.0f;
        totalCarbonEmission = 0.0f;

        for (int i = 0; i < houses.Count; i++)
        {
            if (!houses[i])
            {
                houses.Remove(houses[i]);
                continue;
            }
            taxIncomePerSecond += houses[i].TaxIncome * houses[i].CurrentHappiness;
            averageHappiness += houses[i].CurrentHappiness;
            totalCarbonEmission += houses[i].CarbonEmission / (houses[i].AmountOfBusStops + 1);
        }

        for (int i = 0; i < costlyBuildings.Count; i++)
        {
            taxIncomePerSecond -= costlyBuildings[i].operationCost;
        }

        averageHappiness /= houses.Count;
        averageCarbonEmission = totalCarbonEmission / houses.Count;
        currentMoney += taxIncomePerSecond * Time.deltaTime;

        ProcessLoseConditions();
        
        guiCurrentMoney.SetText($"Fundo: {currentMoney.ToString("0.00")}");
        guiAverageHappiness.SetText($"Felicidade: {averageHappiness.ToString("0.0")}");
        guiAverageCarbonEmission.SetText($"Emissão de CO2: {averageCarbonEmission.ToString("0.0")}");
        guiTaxIncomePerSecond.SetText($"Renda: {taxIncomePerSecond.ToString("0.00")}");

        currentHouseSpawnCooldown += Time.deltaTime;
        if (currentHouseSpawnCooldown >= spawnHouseCooldown)
        {
            SpawnRandomHouse();
            currentHouseSpawnCooldown = 0.0f;
        }
    }

    /// <summary>
    /// Checks if the player is about to lose.
    /// </summary>
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
    
    /// <summary>
    /// Loses the game.
    /// </summary>
    private void LoseGame()
    {
        Time.timeScale = 0.0f;
        CityGUIManager.Instance.SetGameOverWindowVisibility(true);
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

    public void SaveGame()
    {
        House[] housesToSave = FindObjectsOfType<House>();
        PoliceDepartment[] policeDepartmentsToSave = FindObjectsOfType<PoliceDepartment>();
        BusStop[] busStopsToSave = FindObjectsOfType<BusStop>();
        Hospital[] hospitalsToSave = FindObjectsOfType<Hospital>();

        SaveData toSave = new SaveData();
        
        foreach (House house in housesToSave)
        {
            toSave.houses.Add(house.ToSaveData());
        }
        foreach (Hospital hospital in hospitalsToSave)
        {
            toSave.hospitals.Add(hospital.ToSaveData());
        }
        foreach (PoliceDepartment department in policeDepartmentsToSave)
        {
            toSave.policeDepartments.Add(department.ToSaveData());
        }
        foreach (BusStop busStop in busStopsToSave)
        {
            toSave.busStops.Add(busStop.ToSaveData());
        }

        toSave.managerCurrentMoney = currentMoney;
        toSave.timeOfDay = dayNightCycle.CurrentHourOfDay;

        Debug.Log("Saving. Result: " + SaveDataManager.TrySaveData(toSave, 1));
    }

    public bool LoadSaveData()
    {
        var data = SaveDataManager.LoadSaveData(1);

        if (data == null)
        {
            Debug.Log("Save non-existent.");
            return false;
        }
        
        BaseBuilding[] buildingsToDestroy = FindObjectsOfType<BaseBuilding>();
        for (var index = 0; index < buildingsToDestroy.Length; index++)
        {
            BaseBuilding building = buildingsToDestroy[index];
            Destroy(building.gameObject);
        }

        foreach (HouseSaveData houseData in data.houses)
        {
            var house = Instantiate(housePrefab);
            house.transform.position = houseData.position;
            house.transform.eulerAngles = houseData.rotation;
            house.transform.localScale = houseData.scale;

            house.GetComponent<House>().Setup(houseData.carbonEmission, houseData.taxIncome);
        }
        foreach (HospitalSaveData hospitalData in data.hospitals)
        {
            var hospital = Instantiate(hospitalPrefab);
            hospital.transform.position = hospitalData.position;
            hospital.transform.eulerAngles = hospitalData.rotation;
            hospital.transform.localScale = hospitalData.scale;
            
            hospital.GetComponent<Hospital>().Setup(hospitalData.happinessIncrease, hospitalData.secondaryHappinessIncrease);
        }
        foreach (BusStopSaveData busStopData in data.busStops)
        {
            var busStop = Instantiate(busStopPrefab);
            busStop.transform.position = busStopData.position;
            busStop.transform.eulerAngles = busStopData.rotation;
            busStop.transform.localScale = busStopData.scale;
        }
        foreach (PoliceDepartmentSaveData departmentData in data.policeDepartments)
        {
            var policeDepartment = Instantiate(policeDepartmentPrefab);
            policeDepartment.transform.position = departmentData.position;
            policeDepartment.transform.eulerAngles = departmentData.rotation;
            policeDepartment.transform.localScale = departmentData.scale;
            
            policeDepartment.GetComponent<PoliceDepartment>().Setup( departmentData.happinessIncrease, departmentData.secondaryHappinessIncrease);
        }

        currentMoney = data.managerCurrentMoney;
        dayNightCycle.SetTimeHour(data.timeOfDay);

        return true;
    }

    public void TrackCostlyBuildings(ICostlyBuilding building)
    {
        costlyBuildings.Add(building);
    }
}