using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GameObject Variable")]
public class GameObjectVariable : ScriptableObject
{
    [SerializeField]
    private GameObject initialValue = null;

    public GameObject Value { get; set; }

    public void OnEnable()
    {
        Value = initialValue;
    }
}
