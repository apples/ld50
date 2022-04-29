using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnchorPoint : MonoBehaviour
{
    public GameObject poofPrefab;

    public AudioSource sfxPop;

    public float minPopTime;
    public float maxPopTime;

    public float minPopTimeGolden;
    public float maxPopTimeGolden;

    public Vector3 offset;
    public List<Balloon> balloons = new List<Balloon>(8);
    public float balloonTimeUntilPop;

    public int BalloonCount => balloons.Count;

    private new Rigidbody rigidbody;

    public bool IsBalloonPoppingAllowed = true;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (balloons.Count > 0)
        {
            balloonTimeUntilPop -= Time.deltaTime;
            if (balloonTimeUntilPop <= 0)
            {
                PopBalloonAt(0);
                ResetTimer();
            }
        }
    }

    public void GiveBalloon(GameObject obj)
    {
        var balloon = obj.GetComponent<Balloon>();
        balloons.Add(balloon);
        balloon.AnchorTo(rigidbody);

        if (balloons.Count == 1)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        if (balloons.Count > 0 && balloons[0].isGolden)
        {
            balloonTimeUntilPop = Random.Range(minPopTimeGolden, maxPopTimeGolden);
        }
        else
        {
            balloonTimeUntilPop = Random.Range(minPopTime, maxPopTime);
        }
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
        if(!IsBalloonPoppingAllowed){
            return;
        }

        Debug.Assert(i >= 0 && i < BalloonCount);
        var balloon = balloons[i];
        balloons.RemoveAt(i);

        Instantiate(poofPrefab, balloon.transform.position, Quaternion.identity);
        Destroy(balloon.gameObject);

        sfxPop.Play();
    }
}
