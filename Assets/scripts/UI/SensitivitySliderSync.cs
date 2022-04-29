using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySliderSync : MonoBehaviour
{
    private Slider slider;
    public string playerPrefsKey = "aimSensitivity";
    private bool isWoke = false; // somehow the OnValueChanged event is being called before Awake, so this is to ensure we only do the logic there if we've already run Awake

    private void Awake()
    {
        slider = GetComponent<Slider>();
        float savedValue = PlayerPrefs.GetFloat(playerPrefsKey, 10);
        slider.value = savedValue;
        isWoke = true;
    }

    public void ChangePreference(float value)
    {
        if (!isWoke) return;
        PlayerPrefs.SetFloat(playerPrefsKey, value);
        PlayerPrefs.Save();
    }
}
