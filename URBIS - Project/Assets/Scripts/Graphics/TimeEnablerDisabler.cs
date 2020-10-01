using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEnablerDisabler : MonoBehaviour
{
    [SerializeField] private float timeToEnable, timeToDisable;

    [SerializeField] private bool isActiveOnStart;

    [SerializeField] private GameObject target;

    private void Awake()
    {
        DayNightCycle.OnTimeEvent += CheckForTime;
        if (!target)
            target = gameObject;
    }

    private void Start()
    {
        target.SetActive(isActiveOnStart);
    }

    private void OnDestroy()
    {
        DayNightCycle.OnTimeEvent -= CheckForTime;
    }

    private void CheckForTime(DayNightCycle.TimeEventStruct eventinfo)
    {
        if (eventinfo.timeToInvoke.Equals(timeToDisable))
        {
            target.SetActive(false);
        }
        else if (eventinfo.timeToInvoke.Equals(timeToEnable))
        {
            target.SetActive(true);
        }
    }
}
