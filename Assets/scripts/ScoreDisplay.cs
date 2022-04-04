using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private RaftController raftController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private IntScriptableObject maxRaftHeight;
    [SerializeField] private TextMeshProUGUI  textMeshPro;

    void Update()
    {
        textMeshPro.text = String.Format(
            "Fuel time: {0}\n" + 
            "Crates Needed: {1}\n" +
            "Max Altitude: {2}", 
            Mathf.CeilToInt(raftController.FuelTime), scoreManager.crateGoal, maxRaftHeight.value
        );
    }
}
