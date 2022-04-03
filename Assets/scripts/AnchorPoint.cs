using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    public int balloonCount;

    public GameObject BalloonType;

    private List<Balloon> balloons = new();

    private Rigidbody parentRigidbody;

    public float UpwardForce;

    // Start is called before the first frame update
    void Start()
    {
        parentRigidbody = transform.parent.GetComponentInParent<Rigidbody>();
        Debug.Assert(parentRigidbody != null);

        if (BalloonType != null)
        {
            var sphereCollider = BalloonType.GetComponent<SphereCollider>();
            var radius = sphereCollider.radius;
            var incrementAngle = 2 * Mathf.PI / balloonCount;
            for (int i = 0; i < balloonCount; i++)
            {
                var direction = incrementAngle * i;
                var newBalloonObject = Instantiate(BalloonType, transform.position + new Vector3(Mathf.Cos(direction) * radius, 2, Mathf.Sin(direction) * radius), Quaternion.identity, transform);
                var newBalloonScript = newBalloonObject.GetComponent<Balloon>();
                newBalloonScript.IsAnchored = true;
                balloons.Add(newBalloonScript);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        UpwardForce = 100.0f * transform.childCount;
    }
}
