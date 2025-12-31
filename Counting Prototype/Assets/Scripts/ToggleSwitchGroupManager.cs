using System;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSwitchGroupManager : MonoBehaviour
{
    [Header("Start Value")]
    [SerializeField]
    private ToggleSwitch initialToggleSwitch;

    [Header("Toggle Options")]
    [SerializeField]
    private bool allCanBeToggledOff;

    private List<ToggleSwitch> _toggleSwitches = new List<ToggleSwitch>();

    void Awake()
    {
        ToggleSwitch[] toggleSwitches = GetComponentsInChildren<ToggleSwitch>();

        foreach(var t in toggleSwitches)
        {
            RegisterToggleButtonToGroup(t);
        }
    }

    void Start()
    {
        bool areAllToggledOff = true;
        foreach(var btn in _toggleSwitches)
        {
            if(!btn.CurrentValue)
                continue;

            areAllToggledOff = false;
            break;
        }

        if(!areAllToggledOff || allCanBeToggledOff)
            return;

        if(initialToggleSwitch != null)
            initialToggleSwitch.ToggleByGroupManager(true);
        else
            _toggleSwitches[0].ToggleByGroupManager(true);
    }

    private void RegisterToggleButtonToGroup(ToggleSwitch t)
    {
        if(_toggleSwitches.Contains(t))
            return;

        _toggleSwitches.Add(t);

        t.SetupForManager(this);
    }

    public void ToggleGroup(ToggleSwitch toggleSwitch)
    {
        if(_toggleSwitches.Count <= 1)
            return;
        
        if(allCanBeToggledOff && toggleSwitch.CurrentValue)
        {
            foreach(var btn in _toggleSwitches)
            {
                if(btn == null)
                    continue;
                
                btn.ToggleByGroupManager(false);
            }
        }
        else
        {
            foreach(var btn in _toggleSwitches)
            {
                if(btn == null)
                    continue;
                
                if(btn == toggleSwitch)
                    btn.ToggleByGroupManager(true);
                else
                    btn.ToggleByGroupManager(false);
            }
        }
    }
}