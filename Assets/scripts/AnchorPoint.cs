using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnchorPoint : MonoBehaviour
{
    public GameObject poofPrefab;

    public float minPopTime;
    public float maxPopTime;

    public Vector3 offset;
    public List<Balloon> balloons = new List<Balloon>(8);
    public List<float> balloonTimeUntilPop = new List<float>(8);

    public int BalloonCount => balloons.Count;

    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Update()
    {
        for (var i = balloons.Count - 1; i >= 0; --i)
        {
            balloonTimeUntilPop[i] -= Time.deltaTime;
            if (balloonTimeUntilPop[i] <= 0)
            {
                PopBalloonAt(i);
            }
        }
    }

    public void GiveBalloon(GameObject obj)
    {
        var balloon = obj.GetComponent<Balloon>();
        balloons.Add(balloon);
        balloonTimeUntilPop.Add(Random.Range(minPopTime, maxPopTime));
        balloon.AnchorTo(rigidbody);
    }

    public void AdjustBalloonPositions(Vector3 vel)
    {
        foreach (var balloon in balloons)
        {
            balloon.Rigidbody.MovePosition(balloon.Rigidbody.position + vel * Time.deltaTime);
        }
    }

    public void PopBalloonAt(int i)
    {
        Debug.Assert(i >= 0 && i < BalloonCount);
        var balloon = balloons[i];
        balloons.RemoveAt(i);
        balloonTimeUntilPop.RemoveAt(i);

        Instantiate(poofPrefab, balloon.transform.position, Quaternion.identity);
        Destroy(balloon.gameObject);
    }
}
