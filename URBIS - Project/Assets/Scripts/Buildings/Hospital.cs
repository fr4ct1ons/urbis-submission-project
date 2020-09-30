using System;
using UnityEngine;

public class Hospital: BaseBuilding
{
    [SerializeField] private float happinessIncrease = 0.1f;
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;
    [SerializeField] private GameObject radiusObject;
    
    [SerializeField] private CollisionAuxiliary triggerCollider;
    
    [SerializeField] private int connectedHouses = 0;

    public int ConnectedHouses => connectedHouses;

    protected override void Awake()
    {
        base.Awake();
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

            connectedHouses++;
        }
    }

    protected override void OnSelection()
    {
        base.OnSelection();
        radiusObject.SetActive(true);
        CityGUIManager.Instance.ShowHospitalInfo(this);
    }

    public override void Deselect()
    {
        radiusObject.SetActive(false);
    }
}