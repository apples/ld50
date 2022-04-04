using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RaftController : MonoBehaviour
{
    public AnchorPoint anchorPointA1;
    public AnchorPoint anchorPointA2;

    public AnchorPoint anchorPointB1;
    public AnchorPoint anchorPointB2;

    public GameObject bigBalloon;

    public AudioSource sfxFuel;

    public GameObject balloonPrefab;
    public GameObject poofPrefab;

    public float tiltSpringConstant;
    public float tiltDamping;

    public float upwardSpeed;

    public ScoreManager scoreManager;

    public float refuelTime;
    public bool infiniteFuel = false;

    private new Rigidbody rigidbody;

    private float fuelTime;

    public float FuelTime => fuelTime;

    public float NumBalloons =>
        anchorPointA1.BalloonCount +
        anchorPointA2.BalloonCount +
        anchorPointB1.BalloonCount +
        anchorPointB2.BalloonCount;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        Debug.Assert(anchorPointA1 != null);
        Debug.Assert(anchorPointA2 != null);
        Debug.Assert(anchorPointB1 != null);
        Debug.Assert(anchorPointB2 != null);
        Debug.Assert(bigBalloon != null);
        Debug.Assert(balloonPrefab != null);
        Debug.Assert(poofPrefab != null);

        if (scoreManager != null)
        {
            scoreManager.OnCrateGoalReached += this.scoreManager_onCrateGoalReached;
        }

        GiveBalloonTo(anchorPointA1);
        GiveBalloonTo(anchorPointA2);
        GiveBalloonTo(anchorPointB1);
        GiveBalloonTo(anchorPointB2);

        void GiveBalloonTo(AnchorPoint anchorPoint)
        {
            anchorPoint.GiveBalloon(Instantiate(balloonPrefab, anchorPoint.transform.position + Vector3.up, Quaternion.identity));
        }

        fuelTime = refuelTime;
    }

    void Update()
    {
        if(!infiniteFuel){
            fuelTime -= Time.deltaTime;
            if (fuelTime < 0) fuelTime = 0;
        }
    }

    void FixedUpdate()
    {
        ProcessAnchorPoints(anchorPointA1, anchorPointA2);
        ProcessAnchorPoints(anchorPointB1, anchorPointB2);

        var vel = rigidbody.velocity;
        var accel = new Vector3(0, fuelTime > 0 ? upwardSpeed - vel.y : -upwardSpeed * Time.fixedDeltaTime, 0);

        rigidbody.AddForce(accel, ForceMode.VelocityChange);

        anchorPointA1.AdjustBalloonPositions(vel + accel);
        anchorPointA2.AdjustBalloonPositions(vel + accel);
        anchorPointB1.AdjustBalloonPositions(vel + accel);
        anchorPointB2.AdjustBalloonPositions(vel + accel);
    }

    void LateUpdate()
    {
        bigBalloon.transform.rotation = Quaternion.identity;
    }

    private void scoreManager_onCrateGoalReached(object sender, ScoreManager.OnCrateGoalReachedEventArgs e)
    {
        fuelTime += refuelTime;
        foreach (var crate in scoreManager.Crates)
        {
            Instantiate(poofPrefab, crate.transform.position, Quaternion.identity);
        }
        scoreManager.DestroyAllCrates();
        ++scoreManager.crateGoal;

        sfxFuel.Play();
    }

    private void ProcessAnchorPoints(AnchorPoint anchorPoint1, AnchorPoint anchorPoint2)
    {
        var c1 = anchorPoint1.BalloonCount;
        var c2 = anchorPoint2.BalloonCount;

        if (c1 == 0 && c2 == 0) return;

        var d = Mathf.Abs(c1 - c2);

        var a = Mathf.Min(d * 10f, 90f);

        var mn = Mathf.Min(c1, c2);
        var mx = Mathf.Max(c1, c2);

        var desiredAngle =
            mn >= 10 ? Mathf.Min(a, 40f) :
            a <= 40f ? a :
            Mathf.Lerp(40f, a, Mathf.Pow((10f - mn) / 10f, 2));

        Debug.Assert(desiredAngle >= 0f && desiredAngle <= 90f);

        var mnAnchorPoint = mn == c1 ? anchorPoint1 : anchorPoint2;

        var toMn = (mnAnchorPoint.transform.position + mnAnchorPoint.offset - transform.position).normalized;

        var curAngle = Vector3.Angle(Vector3.up, toMn) - 90f;

        var axis = Vector3.Cross(toMn, Vector3.up).normalized;

        var angularVelocity = Vector3.Dot(rigidbody.angularVelocity, axis);

        var angleDeflection = Mathf.Deg2Rad * (curAngle - desiredAngle);

        var torque = axis * (angleDeflection * tiltSpringConstant - angularVelocity * tiltDamping);

        rigidbody.AddTorque(torque);
    }
}
