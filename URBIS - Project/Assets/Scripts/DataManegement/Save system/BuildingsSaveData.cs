using UnityEngine;
using System;

[Serializable]
public class HouseSaveData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    
    public float taxIncome = 1.0f;
    public float currentHappiness = 0.8f;
    public float carbonEmission = 2.0f;
}

[Serializable]
public class BusStopSaveData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[Serializable]
public class HospitalSaveData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    
    public float happinessIncrease = 0.1f;
    public float secondaryHappinessIncrease = 0.05f;
}

[Serializable]
public class PoliceDepartmentSaveData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    
    public float happinessIncrease = 0.1f;
    public float secondaryHappinessIncrease = 0.05f;
}
