using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonVacuum : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        Debug.Log($"Collided with {collider.gameObject}");

        var balloon = collider.gameObject.GetComponent<Balloon>();

        if (balloon != null && !balloon.IsAnchored)
        {
            transform.parent.GetComponent<PlayerController>().GiveItem(balloon.GetComponent<GrapplePoint>());
        }
    }
}
