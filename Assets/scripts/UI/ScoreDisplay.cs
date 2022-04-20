using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// this class is Deprecated by FuelDisplay
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private IntScriptableObject maxRaftHeight;
    [SerializeField] private TextMeshProUGUI  textMeshPro;

    void Update()
    {
        textMeshPro.text =
            $"Max Altitude: {(maxRaftHeight != null ? maxRaftHeight.value : "?")}";
    }
}
