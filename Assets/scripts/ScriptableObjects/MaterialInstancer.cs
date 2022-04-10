using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Material Instancer")]
public class MaterialInstancer : ScriptableObject
{
    public Material sourceMaterial;

    public Material Material { get; private set; }

    void OnEnable()
    {
        Material = sourceMaterial != null ? new Material(sourceMaterial) : null;
    }

    void OnDisable()
    {
        Material = null;
    }
}
