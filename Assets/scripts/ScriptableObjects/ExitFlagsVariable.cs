using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ExitFlags Variable")]
public class ExitFlagsVariable : ScriptableObject
{
    [SerializeField]
    private ExitFlags initialValue;

    public ExitFlags Value { get; set; }

    public void OnEnable()
    {
        Value = initialValue;
    }
}
