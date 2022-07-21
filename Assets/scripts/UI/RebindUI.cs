using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


// this rebinding class was largely inspired by (...or stolen) from this tutorial: https://www.youtube.com/watch?v=TD0R5x0yL0Y

public class RebindUI : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference; // this is on the SO
    [SerializeField] private bool isExcludingMouse;
    [SerializeField] [Range(0, 10)] private int selectedBinding;
    [SerializeField] private InputBinding.DisplayStringOptions displayStringOptions;

    [Header("Binding Info - DO NOT EDIT")] 
    [SerializeField] private InputBinding inputBinding;
    private int bindingIndex;
    private string actionName;

    [Header("UI Fields")] 
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Button rebindButton;
    [SerializeField] private TextMeshProUGUI rebindText;
    [SerializeField] private Button resetButton;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        // resetButton.onClick.AddListener(() => ResetBinding());

        if (inputActionReference == null) return;
        
        GetBindingInfo();
        InputManager.LoadBindingOverride(actionName);
        UpdateUI();

        InputManager.rebindComplete += UpdateUI;
        InputManager.rebindCancelled += UpdateUI;
    }

    private void OnDisable()
    {
        InputManager.rebindCancelled -= UpdateUI;
        InputManager.rebindCancelled -= UpdateUI;
    }

    private void ResetBinding()
    {
        InputManager.ResetBinding(actionName, bindingIndex);
        UpdateUI();
    }

    private void DoRebind()
    {
        InputManager.StartRebind(actionName, bindingIndex, rebindText, isExcludingMouse);
    }

    private void OnValidate()
    {
        if (inputActionReference == null) return;
        GetBindingInfo();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (actionText != null) actionText.text = actionName;

        if (rebindText == null) return;
        if (Application.isPlaying)
        {
            rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
        }
        else
        {
            rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
        }
    }

    private void GetBindingInfo()
    {
        if (inputActionReference.action == null) return;
        actionName = inputActionReference.action.name;
        
        if (inputActionReference.action.bindings.Count <= selectedBinding) return;
        inputBinding = inputActionReference.action.bindings[selectedBinding];
        bindingIndex = selectedBinding;
    }
}
