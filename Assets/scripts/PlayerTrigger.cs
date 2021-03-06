using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent<GameObject> onPlayerEnter = new UnityEvent<GameObject>();
    public UnityEvent<GameObject> onPlayerExit = new UnityEvent<GameObject>();

    void OnTriggerEnter(Collider collider)
    {
        //Debug.Log($"PlayerTrigger collided with {collider.gameObject}");
        if (collider.gameObject.CompareTag("Player"))
        {
            onPlayerEnter.Invoke(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        //Debug.Log($"PlayerTrigger exited {collider.gameObject}");
        if (collider.gameObject.CompareTag("Player"))
        {
            onPlayerExit.Invoke(collider.gameObject);
        }
    }
}
