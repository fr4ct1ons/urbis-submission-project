using System;
using UnityEngine;

public class Hospital: BaseBuilding , ICostlyBuilding
{
    [SerializeField] private float happinessIncrease = 0.1f;
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;
    [SerializeField] private GameObject radiusObject;

    [SerializeField] private CollisionAuxiliary triggerCollider;
    
    [SerializeField] private int connectedHouses = 0;

    [SerializeField] private float operationCostPerSecond = 1.0f;

    public float operationCost
    {
        get => operationCostPerSecond;
        set => operationCostPerSecond = value;
    }

    public int ConnectedHouses => connectedHouses;

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

    public HospitalSaveData ToSaveData()
    {
        HospitalSaveData data = new HospitalSaveData();
        
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