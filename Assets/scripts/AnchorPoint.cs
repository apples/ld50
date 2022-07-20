using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public float maxInflationScale;
    public float pulseTimeScale;
    public float inflationStartTime;
    public float inflationPulseTime;
    public float inflationStartTimeGolden;
    public float inflationPulseTimeGolden;

    public Vector3 offset;
    public List<Balloon> balloons = new List<Balloon>(8);
    public float balloonTimeUntilPop;

    public int BalloonCount => balloons.Count;

    public float BalloonFactor(float normalFactor, float goldenFactor) =>
        balloons.Count(x => x.isGolden) * goldenFactor +
        balloons.Count(x => !x.isGolden) * normalFactor;

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
            else
            {
                var balloon = balloons[0];
                var startTime = balloon.isGolden ? inflationStartTimeGolden : inflationStartTime;
                var pulseTime = balloon.isGolden ? inflationPulseTimeGolden : inflationPulseTime;
                var inflation =
                    balloonTimeUntilPop < pulseTime ? (Mathf.Cos((pulseTime - balloonTimeUntilPop) * pulseTimeScale) * 0.5f + 0.5f) :
                    balloonTimeUntilPop < startTime ? (startTime - balloonTimeUntilPop) / (startTime - pulseTime) :
                    0f;
                var inflationScale = Mathf.Lerp(1f, maxInflationScale, inflation);

                balloons[0].balloonMesh.localScale = new Vector3(
                    inflationScale,
                    inflationScale,
                    inflationScale
                );
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
