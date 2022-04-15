using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventListener : MonoBehaviour
{
    public VoidEvent voidEvent;

    public UnityEvent response;

    void OnEnable()
    {
        voidEvent.AddListener(this);
    }

    void OnDisable()
    {
        voidEvent.RemoveListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }
}
