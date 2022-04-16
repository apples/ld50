using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject target;
    public bool x;
    public bool y;
    public bool z;

    void Start()
    {
        Debug.Assert(target != null);
    }

    void Update()
    {
        var pos = transform.position;
        if (x) pos.x = target.transform.position.x;
        if (y) pos.y = target.transform.position.y;
        if (z) pos.z = target.transform.position.z;
        transform.position = pos;
    }
}
