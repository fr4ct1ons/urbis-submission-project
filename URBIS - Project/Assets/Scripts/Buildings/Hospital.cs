using System;
using UnityEngine;

public class Hospital: BaseBuilding
{
    [SerializeField] private float happinessIncrease = 0.1f;
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;

    [SerializeField] private CollisionAuxiliary triggerCollider;

    private void Awake()
    {
        triggerCollider.TriggerEnter += OnTriggerEnter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            if (!house.HasHospital)
            {
                house.CurrentHappiness += happinessIncrease;
                house.HasHospital = true;
            }
            else
            {
                house.CurrentHappiness += secondaryHappinessIncrease;
            }
        }
    }
}