using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : BaseBuilding
{
    [Tooltip("Tax income of this house per second.")]
    [SerializeField] private float taxIncome = 1.0f;

    public float TaxIncome => taxIncome;

    private void Start()
    {
        if (CityManager.Instance)
        {
            CityManager.Instance.TrackHouse(this);
        }
        else
        {
            Debug.LogError("CityManager instance not set!");
        }
    }
}
