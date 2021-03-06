using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Int Variable")]
public class IntScriptableObject : ScriptableObject
{
    public int value;

    public void SetValue(float val)
    {
        value = (int) val;
    }
}
