using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstanceLoader : MonoBehaviour
{
    public MaterialInstancer materialInstancer;
    public Renderer rendererComponent;

    void Start()
    {
        rendererComponent.material = materialInstancer.Material;
    }
}
