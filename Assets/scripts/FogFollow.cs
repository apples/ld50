using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFollow : MonoBehaviour
{
    public Rigidbody objectToFollow;
    public Rigidbody fogPlane;

    public float minimumY;
    public float followDist;

    public bool freezeFog;

    private float raftHeightReached;

    void Start()
    {
        Debug.Assert(objectToFollow != null);
        Debug.Assert(fogPlane != null);

        raftHeightReached = objectToFollow.transform.position.y;
    }

    void FixedUpdate()
    {
        raftHeightReached = Mathf.Max(raftHeightReached, objectToFollow.transform.position.y);

        if (!freezeFog)
        {
            var fogPos = fogPlane.transform.position;
            fogPos.y = Mathf.Max(minimumY, raftHeightReached - followDist);
            fogPlane.transform.position = fogPos;
        }
    }
}
