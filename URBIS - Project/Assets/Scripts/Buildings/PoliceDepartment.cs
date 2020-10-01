using System;
using UnityEngine;

public class PoliceDepartment: BaseBuilding, ICostlyBuilding
{
    [SerializeField] private float happinessIncrease = 0.1f;
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;
    
    [SerializeField] private CollisionAuxiliary triggerCollider;
    
    [SerializeField] private GameObject radiusObject;
    [SerializeField] private int connectedHouses = 0;
    
    [SerializeField] private float operationCostPerSecond = 1.0f;

    public float operationCost
    {
        get => operationCostPerSecond;
        set => operationCostPerSecond = value;
    }
    
    public int ConnectedHouses
    {
        get => connectedHouses;
        set => connectedHouses = value;
    }

    protected override void Awake()
    {
        base.Awake();
        triggerCollider.TriggerEnter += OnTriggerEnter;
    }
    
    protected override void Start()
    {
        base.Start();
        CityManager.Instance.TrackCostlyBuildings(this);
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<House>(out House house))
        {
            if (!house.HasPoliceDepartment)
            {
                house.CurrentHappiness += happinessIncrease;
                house.HasPoliceDepartment = true;
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
        CityGUIManager.Instance.ShowPoliceDepartmentInfo(this);
    }

    public override void Deselect()
    {
        radiusObject.SetActive(false);
    }
    
    public PoliceDepartmentSaveData ToSaveData()
    {
        PoliceDepartmentSaveData data = new PoliceDepartmentSaveData();
        
        data.position = transform.position;
        data.rotation = transform.eulerAngles;
        data.scale = transform.lossyScale;

        data.happinessIncrease = happinessIncrease;
        data.secondaryHappinessIncrease = secondaryHappinessIncrease;

        return data;
    }

    public void Setup(float newHappinessIncrease, float newSecondaryHappinessIncrease)
    {
        happinessIncrease = newHappinessIncrease;
        secondaryHappinessIncrease = newSecondaryHappinessIncrease;
    }
}
