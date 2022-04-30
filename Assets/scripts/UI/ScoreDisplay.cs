using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
