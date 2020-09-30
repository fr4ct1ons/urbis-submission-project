using System;
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
    [Tooltip("Time between spawning a random house.")]
    [SerializeField] private float spawnHouseCooldown = 5.0f;
    
    [Space]
    
    [Tooltip("Object that will be spawned at empty tiles.")]
    [SerializeField] private GameObject emptyTileHighlight;

    [SerializeField] private GameObject housePrefab;
    [SerializeField] private GameObject hospitalPrefab;
    [SerializeField] private GameObject policeDepartmentPrefab;
    [SerializeField] private GameObject busStopPrefab;

    [Space]
    
    [SerializeField] private TextMeshProUGUI guiCurrentMoney;
    [SerializeField] private TextMeshProUGUI guiTaxIncomePerSecond;
    [SerializeField] private TextMeshProUGUI guiAverageHappiness;
    [SerializeField] private TextMeshProUGUI guiAverageCarbonEmission;

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

        averageCarbonEmission = totalCarbonEmission;
        
        currentMoney += taxIncomePerSecond * Time.deltaTime;
        
        guiCurrentMoney.SetText(currentMoney.ToString("0.00"));
        guiAverageHappiness.SetText(averageHappiness.ToString("0.0"));
        guiAverageCarbonEmission.SetText(averageCarbonEmission.ToString("0.0"));
        guiTaxIncomePerSecond.SetText(averageCarbonEmission.ToString("0.00"));

        currentHouseSpawnCooldown += Time.deltaTime;
        if (currentHouseSpawnCooldown >= spawnHouseCooldown)
        {
            SpawnRandomHouse();
            currentHouseSpawnCooldown = 0.0f;
        }
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
        switch (currentBuildingState)
        {
            case BuildingHouseState.BuildingHospital:
                Instantiate(hospitalPrefab, tile.transform.position, Quaternion.identity);
                emptyTiles.Remove(tile);
                Destroy(tile.gameObject);
                break;
            case BuildingHouseState.BuildingPoliceDepartment:
                Instantiate(policeDepartmentPrefab, tile.transform.position, Quaternion.identity);
                emptyTiles.Remove(tile);
                Destroy(tile.gameObject);
                break;
            case BuildingHouseState.BuildingBusStop:
                Instantiate(busStopPrefab, tile.transform.position, Quaternion.identity);
                emptyTiles.Remove(tile);
                Destroy(tile.gameObject);
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