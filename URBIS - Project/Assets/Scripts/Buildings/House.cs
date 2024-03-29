﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : BaseBuilding
{
    [Tooltip("Tax income of this house per second.")]
    [SerializeField] private float taxIncome = 1.0f;
    [Tooltip("Happiness in the current house. The house's taxIncome value will be multiplied by the happiness")]
    [SerializeField] private float currentHappiness = 0.8f;
    [Tooltip("Carbon gas normally emitted by the house.")]
    [SerializeField] private float carbonEmission = 2.0f;
    
    [Tooltip("Variables serialized for easy editability.")]
    [SerializeField] private bool hasPoliceDepartment = false;
    [SerializeField] private bool hasHospital = false;
    [SerializeField] private int amountOfBusStops = 0;

    public float CarbonEmission
    {
        get => carbonEmission;
        set => carbonEmission = value;
    }
    
    public int AmountOfBusStops
    {
        get => amountOfBusStops;
        set => amountOfBusStops = value;
    }

    public bool HasHospital
    {
        get => hasHospital;
        set => hasHospital = value;
    }

    public bool HasPoliceDepartment
    {
        get => hasPoliceDepartment;
        set => hasPoliceDepartment = value;
    }
    
    public float CurrentHappiness
    {
        get => currentHappiness;
        set => currentHappiness = value;
    }
    
    public float TaxIncome => taxIncome;

    protected override void Start()
    {
        base.Start();
        
        if (CityManager.Instance)
        {
            CityManager.Instance.TrackHouse(this);
        }
        else
        {
            Debug.LogError("CityManager instance not set!");
        }
    }

    /// <summary>
    /// Shows the house info such as carbon emission, happiness etc.
    /// </summary>
    protected override void OnSelection()
    {
        CityGUIManager.Instance.ShowHouseInfo(this);
    }

    public HouseSaveData ToSaveData()
    {
        HouseSaveData data = new HouseSaveData();
        
        data.position = transform.position;
        data.rotation = transform.eulerAngles;
        data.scale = transform.lossyScale;

        data.carbonEmission = carbonEmission;
        data.taxIncome = taxIncome;
        data.currentHappiness = currentHappiness;

        return data;
    }

    public void Setup(float newCarbonEmission, float newTaxIncome)
    {
        carbonEmission = newCarbonEmission;
        taxIncome = newTaxIncome;
    }
}
