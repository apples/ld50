using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogPlane : MonoBehaviour
{
    public int playerLayer;
    public int balloonLayer;
    public float playerBounceSpeed;

    void OnTriggerEnter(Collider collider)
    {
        Process(collider.attachedRigidbody);
    }

    void OnTriggerStay(Collider collider)
    {
        Process(collider.attachedRigidbody);
    }

    private void Process(Rigidbody rigidbody)
    {
        if (rigidbody.gameObject.layer == playerLayer && rigidbody.velocity.y < playerBounceSpeed)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, playerBounceSpeed, rigidbody.velocity.z);
        }
        else if (rigidbody.gameObject.layer == balloonLayer)
        {
            if (rigidbody.gameObject.GetComponent<Balloon>() is Balloon balloon)
            {
                if (!balloon.IsAnchored)
                {
                    Destroy(rigidbody.gameObject);
                }
            }
        }
    }
}
