using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehavior : MonoBehaviour
{
    public int crateLayer;
    public int crateMovingUpLayer;

    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rigidbody.velocity.y <= 0)
        {
            gameObject.layer = crateLayer;
        }
        else
        {
            gameObject.layer = crateMovingUpLayer;
        }
    }
}
