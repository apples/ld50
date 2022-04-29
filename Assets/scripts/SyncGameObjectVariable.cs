using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncGameObjectVariable : MonoBehaviour
{
    public GameObjectVariable gameObjectVariable;

    void OnEnable()
    {
        gameObjectVariable.Value = this.gameObject;
    }

    void OnDisable()
    {
        gameObjectVariable.Value = null;
    }
}
