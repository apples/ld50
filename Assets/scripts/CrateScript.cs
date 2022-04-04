using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    private int cloudPhysicsLayer = 7;
    private int cratePhysicsLayer = 8;
    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void FixedUpdate()
    {
        Physics.IgnoreLayerCollision(cratePhysicsLayer, cloudPhysicsLayer, rigidbody.velocity.y > 0);
    }
}
