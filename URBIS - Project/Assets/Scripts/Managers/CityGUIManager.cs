using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class CityGUIManager : MonoBehaviour
{
    private static CityGUIManager instance;

    public static CityGUIManager Instance => instance;

    [SerializeField] private bool isPersistent;

    [Space]
    
    [SerializeField] private GameObject structureInfoWindow;
    [SerializeField] private TextMeshProUGUI structureInfoText;
    [SerializeField] private GameObject gameOverWindow;
    
    [Space]
    [SerializeField] private bool isTimePaused;

    [SerializeField] private UnityEvent OnPauseTime;
    [SerializeField] private UnityEvent OnResumeTime;

    private BaseBuilding selectedObject;

    public BaseBuilding SelectedObject
    {
        get => selectedObject;
        //set => selectedObject = value;
    }
    
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            if(isPersistent)
                DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void BeforeShowInfo()
    {
        if (selectedObject)
        {
            selectedObject.Deselect();
        }
    }

    public void HideInfoWindow()
    {
        selectedObject.Deselect();
        selectedObject = null;
        structureInfoWindow.SetActive(false);
    }

    public void ShowHouseInfo(House house)
    {
        BeforeShowInfo();
        selectedObject = house;
        structureInfoWindow.SetActive(true);
        string temp = "Casa\n" +
                      "Renda: " + house.TaxIncome + "\n" +
                      "Felicidade: " + house.CurrentHappiness + "\n" +
                      "Emissão de CO2: " + house.CarbonEmission + "\n";
        if (!house.HasHospital)
        {
            temp += "<color=red>Não há acesso a um hospital!</color>\n";
        }
        if (!house.HasPoliceDepartment)
        {
            temp += "<color=red>Não há acesso a uma delegacia!</color>!";
        }
        structureInfoText.SetText(temp);
    }

    public void ShowHospitalInfo(Hospital hospital)
    {
        selectedObject = hospital;
        structureInfoWindow.SetActive(true);
        string temp = "Hospital\n" +
                      "Casas conectadas: " + hospital.ConnectedHouses;

        structureInfoText.SetText(temp);
    }

    public void ShowBusStopInfo(BusStop busStop)
    {
        selectedObject = busStop;
        structureInfoWindow.SetActive(true);
        string temp = "Parada de ônibus\n" +
                      "Casas conectadas: " + busStop.ConnectedHouses;

        structureInfoText.SetText(temp);
    }
    
    public void ShowPoliceDepartmentInfo(PoliceDepartment policeDepartment)
    {
        selectedObject = policeDepartment;
        structureInfoWindow.SetActive(true);
        string temp = "Delegacia\n" +
                      "Casas conectadas: " + policeDepartment.ConnectedHouses;

        structureInfoText.SetText(temp);
    }

    public void SetGameOverWindowVisibility(bool visibility)
    {
        gameOverWindow.SetActive(visibility);
    }

    public void PauseTime()
    {
        switch (isTimePaused)
        {
            case true:
                Time.timeScale = 1.0f;
                isTimePaused = false;
                OnResumeTime.Invoke();
                break;
            case false:
                Time.timeScale = 0.0f;
                isTimePaused = true;
                OnPauseTime.Invoke();
                break;
        }
    }
}