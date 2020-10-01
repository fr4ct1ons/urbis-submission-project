using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
    [Serializable]
    public class TimeEventStruct
    {
        public string eventName;
        [Range(0.0f, 24.0f)] public float timeToInvoke;
        public bool wasInvoked;
        public UnityEvent OnReachTime;
    }
    
    
    [Tooltip("Reference to the sun in the scene. If none is passed, it will get automatically from RenderSettings.")]
    [SerializeField] private Light sun;
    [Tooltip("Current hour of the day. Whenever updating the ambient time , this value will always be repeated between 0 and 24.")]
    [SerializeField, Range(0.0f, 24.0f)] private float currentHourOfDay;
    [Tooltip("Number of minutes it takes to complete a full day-night cycle. Set to zero and this value will not be updated automatically.")]
    [SerializeField] private float minutesForCycle = 1.0f;
    
    [Space]
    [Tooltip("The ambient color during the day-night cycle.")]
    [SerializeField] private Gradient ambientColorGradient;
    [Tooltip("The sun color during the day-night cycle.")]
    [SerializeField] private Gradient sunColorGradient;
    [Tooltip("The skybox's exposure during the day-night cycle. Only the times between 0 and 1 will be read.")]
    [SerializeField] private AnimationCurve skyboxExposureCurve;
    [Tooltip("The sun's intensity during the cycle.")]
    [SerializeField] private AnimationCurve sunIntensityCurve = AnimationCurve.Constant(0, 1, 3.0f);

    [Tooltip("Events to be invoked. The events will only be invoked if Minutes for Cycle is different than zero.")]
    [SerializeField] private List<TimeEventStruct> events = new List<TimeEventStruct>();

    [Header("The below values were serialized only for being easy to view their values during runtime.")]
    [SerializeField] private float timePercentage = 0.0f;

    private float currentTimeOfDay = 0.0f;
    private float previousFrameHour = 0.0f;

    public delegate void CycleDelegate(TimeEventStruct eventInfo);

    public static event CycleDelegate OnTimeEvent;

    public float CurrentHourOfDay => currentHourOfDay;
    
    private void Awake()
    {
        if (!sun)
            sun = RenderSettings.sun;

        currentTimeOfDay = currentHourOfDay / 24.0f;
    }

    private void Start()
    {
        SetTimeHour(currentHourOfDay);
    }

    private void Update()
    {
        if (minutesForCycle != 0.0f)
        {
            currentTimeOfDay += (Time.deltaTime / 60.0f) / minutesForCycle;
            SetTimePercentage(currentTimeOfDay);

            float lastFrameTime = Mathf.Repeat(currentHourOfDay - Time.deltaTime, 24.0f);
            float currentFrameTime = Mathf.Repeat(currentHourOfDay, 24.0f);
            if ((lastFrameTime <= 24.0f && lastFrameTime >= 23.25f) &&
                (currentFrameTime >= 0.0f && currentFrameTime <= 0.25f)) // if just became midnight
            {
                Debug.Log("Midnight");
                foreach (TimeEventStruct eventStruct in events)
                {
                    eventStruct.wasInvoked = false;
                }
            }
                
            for (int i = 0; i < events.Count; i++)
            {
                if (currentHourOfDay >= events[i].timeToInvoke)
                {
                    if (!events[i].wasInvoked)
                    {
                        OnTimeEvent?.Invoke(events[i]);
                        events[i].OnReachTime.Invoke();
                        events[i].wasInvoked = true;
                    }
                }
            }
        }
    }

    public void SetTimeHour(float hourOfDay)
    {
        currentHourOfDay = Mathf.Repeat(hourOfDay, 24.0f);
        timePercentage = (currentHourOfDay/ 24.0f);
        sun.transform.localRotation = Quaternion.Euler(new Vector3(360.0f * timePercentage, 0, 0));
        
        sun.color = sunColorGradient.Evaluate(timePercentage);
        sun.intensity = sunIntensityCurve.Evaluate(timePercentage);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(timePercentage);
        //RenderSettings.skybox.SetFloat("_Exposure", skyboxExposureCurve.Evaluate(timePercentage));
        RenderSettings.ambientIntensity = skyboxExposureCurve.Evaluate(timePercentage);
    }

    public void SetTimePercentage(float percentual)
    {
        timePercentage = Mathf.Repeat(percentual, 1.0f);
        currentHourOfDay = 24 * timePercentage;
        sun.transform.localRotation = Quaternion.Euler(new Vector3(360.0f * timePercentage, 0, 0));
        
        sun.color = sunColorGradient.Evaluate(timePercentage);
        RenderSettings.ambientLight = ambientColorGradient.Evaluate(timePercentage);
        RenderSettings.ambientIntensity = skyboxExposureCurve.Evaluate(timePercentage);
        //RenderSettings.skybox.SetFloat("_Exposure", skyboxExposureCurve.Evaluate(timePercentage));
    }

    [ContextMenu("Update time")]
    public void EditorSetAmbientTime()
    {
        SetTimeHour(currentHourOfDay);
    }
}
