using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    public Vector3 offset;
    public List<Balloon> balloons = new List<Balloon>();

    public int BalloonCount => balloons.Count;

    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void FixedUpdate()
    {
        ApplyAnchorVelocityToBalloons();
    }

    public void GiveBalloon(GameObject obj)
    {
        var balloon = obj.GetComponent<Balloon>();
        balloons.Add(balloon);
        balloon.AnchorTo(rigidbody);
    }

    public void ApplyAnchorVelocityToBalloons()
    {
        foreach (var balloon in balloons)
        {
            balloon.Rigidbody.MovePosition(balloon.Rigidbody.position + rigidbody.velocity * Time.deltaTime);
        }
    }
}
