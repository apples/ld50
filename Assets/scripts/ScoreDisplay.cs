using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private RaftController raftController;
    [SerializeField] private IntScriptableObject maxRaftHeight;
    [SerializeField] private TextMeshProUGUI  textMeshPro;

    void Update()
    {
        textMeshPro.text = String.Format("Fuel time: {0}\nMax Altitude: {1}", Mathf.CeilToInt(raftController.FuelTime), maxRaftHeight.value);
    }
}
