using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// this class is Deprecated by FuelDisplay
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private RaftController raftController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private IntScriptableObject maxRaftHeight;
    [SerializeField] private TextMeshProUGUI  textMeshPro;

    void Update()
    {
        textMeshPro.text =
            $"Fuel time: {(raftController != null ? Mathf.CeilToInt(raftController.FuelTime) : "?")}\n" + 
            $"Crates Needed: {(scoreManager != null ? scoreManager.crateGoal : "?")}\n" +
            $"Max Altitude: {(maxRaftHeight != null ? maxRaftHeight.value : "?")}";
    }
}
