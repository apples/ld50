using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Void Event")]
public class VoidEvent : ScriptableObject
{
    private List<VoidEventListener> listeners = new List<VoidEventListener>(4);

    public void AddListener(VoidEventListener listener)
    {
        Debug.Assert(!listeners.Contains(listener));
        listeners.Add(listener);
    }

    public void RemoveListener(VoidEventListener listener)
    {
        Debug.Assert(listeners.Contains(listener));
        listeners.Remove(listener);
    }

    public void Raise()
    {
        foreach (var listener in listeners)
        {
            listener.OnEventRaised();
        }
    }
}
