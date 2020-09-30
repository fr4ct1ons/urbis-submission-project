using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    private static CityManager instance;

    public static CityManager Instance => instance;

    [Tooltip("Mark the object as DontDestroyOnLoad.")]
    [SerializeField] private bool isPersistent;
    [Tooltip("Money to be received at the start of the game.")]
    [SerializeField] private float startingMoney = 500;
    
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
    
    private HashSet<House> houses = new HashSet<House>();

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
    }

    private void Update()
    {
        taxIncomePerSecond = 0.0f;
        averageHappiness = 0.0f;
        totalCarbonEmission = 0.0f;
        
        foreach (House house in houses)
        {
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
    }

    public void TrackHouse(House house)
    {
        houses.Add(house);
    }
}