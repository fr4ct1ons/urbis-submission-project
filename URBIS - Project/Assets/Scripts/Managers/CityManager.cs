using System;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    private static CityManager instance;

    public static CityManager Instance => instance;

    [Tooltip("Mark the object as DontDestroyOnLoad.")]
    [SerializeField] private bool isPersistent;

    [SerializeField] private HashSet<House> houses = new HashSet<House>();

    [SerializeField] private float currentMoney;

    private void Awake()
    {
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
            currentMoney += house.TaxIncome * Time.deltaTime;
        }
    }

    public void TrackHouse(House house)
    {
        houses.Add(house);
    }
}