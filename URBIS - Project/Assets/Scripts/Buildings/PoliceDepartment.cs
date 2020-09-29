using System;
using UnityEngine;

public class PoliceDepartment: BaseBuilding
{
    [SerializeField] private float happinessIncrease = 0.1f;
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            if (house.HasPoliceDepartment)
            {
                house.CurrentHappiness += happinessIncrease;
                house.HasPoliceDepartment = true;
            }
            else
            {
                house.CurrentHappiness += secondaryHappinessIncrease;
            }
        }
    }
}
