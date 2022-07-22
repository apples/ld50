using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorReticle : MonoBehaviour
{
    public Transform target;

    public float speed;

    private Vector3 actualPosition;

    void OnEnable()
    {
        if (target != null)
        {
            transform.position = target.transform.position;
            actualPosition = transform.position;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            var dv = target.position - actualPosition;

            var dist = Mathf.Min(speed * Time.deltaTime, dv.magnitude);

            actualPosition += dv.normalized * dist;

            transform.position = actualPosition;
        }
    }
}
