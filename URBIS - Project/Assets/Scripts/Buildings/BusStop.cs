using System;
using UnityEngine;

public class BusStop: BaseBuilding
{
    [SerializeField] private float carbonDecrease = 3.0f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            house.AmountOfBusStops++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            house.AmountOfBusStops--;
        }
    }
}