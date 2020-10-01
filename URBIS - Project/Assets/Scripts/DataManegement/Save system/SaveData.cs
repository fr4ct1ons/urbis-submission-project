using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int slotNumber; //Starts at one. Index would start at zero.
    public float timeOfDay;
    
    public float managerCurrentMoney;

    public List<HouseSaveData> houses = new List<HouseSaveData>();
    public List<PoliceDepartmentSaveData> policeDepartments = new List<PoliceDepartmentSaveData>();
    public List<BusStopSaveData> busStops = new List<BusStopSaveData>();
    public List<HospitalSaveData> hospitals = new List<HospitalSaveData>();
}