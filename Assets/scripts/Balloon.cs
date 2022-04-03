using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float minFloatDirectionTime = 1f;
    public float maxFloatDirectionTime = 3f;
    public float minFloatSpeed;
    public float maxFloatSpeed;
    public float springMaxDist = 1f;

    private float floatDirectionTime;
    private float floatDirectionTimer;
    private Vector3 floatDirection;
    private float floatSpeed;

    private bool isAnchored = false;

    private new Rigidbody rigidbody;
    private SpringJoint spring;
    private GrapplePoint grapplePoint;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);

        grapplePoint = GetComponent<GrapplePoint>();
        Debug.Assert(grapplePoint != null);
    }

    void Start()
    {
        if (isAnchored == false)
        {
            DetermineFloatPath();
        }
    }

    void Update()
    {
        if (isAnchored)
        {

        }
        else
        {
            floatDirectionTimer += Time.deltaTime;
            if(floatDirectionTimer >= floatDirectionTime){
                DetermineFloatPath();
            }
        }
    }

    private void DetermineFloatPath()
    {
        floatDirectionTimer = 0f;
        floatDirectionTime = Random.Range(minFloatDirectionTime, maxFloatDirectionTime);

        floatDirection = new Vector3(Random.Range(-.1f,1), Random.Range(-.3f,1), Random.Range(-1,1)).normalized;
        floatSpeed = Random.Range(minFloatSpeed, maxFloatSpeed);

        rigidbody.velocity = floatDirection * floatSpeed;
    }

    public void AnchorTo(Rigidbody connectedRigidbody)
    {
        if (connectedRigidbody != null)
        {
            Debug.Assert(isAnchored == false);
            Debug.Assert(spring == null);
            isAnchored = true;
            spring = gameObject.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedBody = connectedRigidbody;
            spring.connectedAnchor = Vector3.zero;
            spring.maxDistance = springMaxDist;
            spring.connectedMassScale = 0.00001f;

            grapplePoint.disabled = true;
        }
        else
        {
            Debug.Assert(isAnchored == true);
            Debug.Assert(spring != null);
            isAnchored = false;
            Destroy(spring);
            spring = null;

            grapplePoint.disabled = false;
        }
    }
}
