using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOrDeselectBasedOnInput : MonoBehaviour
{
    public EventSystem eventSystem;
    public Selectable focusedSelectable;

    private PlayerInputActions playerInputActions;
    private string currentDeviceName;

    private void Start() 
    {
        playerInputActions = InputManager.inputActions;
        playerInputActions.Player.Enable();    
    }

    private void OnDisable()
    {
        currentDeviceName = null;
    }

    void Update()
    {
        if (!focusedSelectable)
        {
            return;
        }
        
        if(playerInputActions.Player.UINavigation.activeControl == null){
            return;
        }
        
        string deviceName = playerInputActions.Player.UINavigation.activeControl.device.name;

        if(deviceName == currentDeviceName){
            return;
        }
        
        currentDeviceName = deviceName;

        if (currentDeviceName == "Mouse")
        {
            eventSystem.SetSelectedGameObject(null);
        }
        else
        {
            eventSystem.SetSelectedGameObject(focusedSelectable.gameObject);
        }
    }
}
