using System;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    private static CityManager instance;

    public static CityManager Instance => instance;

    [Tooltip("Mark the object as DontDestroyOnLoad.")]
    [SerializeField] private bool isPersistent;
    
    [SerializeField] private float startingMoney = 500;
    [SerializeField] private float startingCarbonEmission = 15;
    [SerializeField] private float maxCarbonEmission = 30;
    
    [Header("The below variables are serialized only for being easily viewable on the editor.")]
    
    [SerializeField] private float currentMoney;
    [SerializeField] private float currentCarbonEmission;

    private HashSet<House> houses = new HashSet<House>();

    private float taxIncomePerSecond;
    private void Awake()
    {
        currentMoney = startingMoney;
        currentCarbonEmission = startingCarbonEmission;
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
        foreach (House house in houses)
        {
            taxIncomePerSecond += house.TaxIncome * house.CurrentHappiness;
        }

        taxIncomePerSecond /= (maxCarbonEmission / 2)+ currentCarbonEmission/maxCarbonEmission;
        taxIncomePerSecond *= Time.deltaTime;
        currentMoney += taxIncomePerSecond;
    }

    public void TrackHouse(House house)
    {
        houses.Add(house);
    }
}