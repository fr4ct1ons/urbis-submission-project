using System;
using UnityEngine;

public class BusStop: BaseBuilding
{
    [SerializeField] private float carbonDecrease = 3.0f;
    
    [SerializeField] private CollisionAuxiliary auxCollider;

    private void Awake()
    {
        auxCollider.TriggerEnter += OnAuxEnter;
        auxCollider.TriggerExit += OnAuxExit;
    }
    
    private void OnAuxEnter(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            house.AmountOfBusStops++;
        }
    }

    private void OnAuxExit(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            house.AmountOfBusStops--;
        }
    }
}