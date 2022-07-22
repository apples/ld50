using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TetherSensor : MonoBehaviour
{
    public UnityEvent<AnchorPoint> onEnter;
    public UnityEvent<AnchorPoint> onExit;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<AnchorPoint>() is AnchorPoint ap)
        {
            onEnter?.Invoke(ap);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<AnchorPoint>() is AnchorPoint ap)
        {
            onExit?.Invoke(ap);
        }
    }
}
