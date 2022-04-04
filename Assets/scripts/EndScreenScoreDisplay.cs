using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class EndScreenScoreDisplay : MonoBehaviour
{
    public IntScriptableObject maxRaftHeight;
    private TextMeshProUGUI tmpText;

    private void Start() {
        tmpText = GetComponent<TextMeshProUGUI>();
        tmpText.text = String.Format("{0} METERS!", maxRaftHeight.value.ToString());

        Cursor.lockState = CursorLockMode.None;
    }
}
