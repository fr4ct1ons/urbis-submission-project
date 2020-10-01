using System;
using UnityEngine;

public class PoliceDepartment: BaseBuilding, ICostlyBuilding
{
    [Tooltip("Happiness to be increased in the house if it does not have a hospital.")]
    [SerializeField] private float happinessIncrease = 0.1f;
    [Tooltip("Happiness to be increased in the house if it has a hospital.")]
    [SerializeField] private float secondaryHappinessIncrease = 0.05f;
    [Tooltip("Object that shows the hospital range.")]
    [SerializeField] private GameObject radiusObject;
    [Tooltip("How much tax income the hospital needs to operate.")]
    [SerializeField] private float operationCostPerSecond = 1.0f;
    
    [SerializeField] private int connectedHouses = 0;
    [SerializeField] private CollisionAuxiliary triggerCollider;

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
    
    /// <summary>
    /// Increases happiness on the collided house.
    /// </summary>
    /// <param name="other"></param>
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
    
    /// <summary>
    /// Enables RadiusObject.
    /// </summary>
    protected override void OnSelection()
    {
        base.OnSelection();
        radiusObject.SetActive(true);
        CityGUIManager.Instance.ShowPoliceDepartmentInfo(this);
    }

    /// <summary>
    /// Disables RadiusObject.
    /// </summary>
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
