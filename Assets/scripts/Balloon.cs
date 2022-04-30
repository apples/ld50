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

    public bool isGolden;

    public Transform balloonMesh;

    private float floatDirectionTime;
    private float floatDirectionTimer;
    private Vector3 floatDirection;
    private float floatSpeed;

    private new Rigidbody rigidbody;
    private SpringJoint spring;
    private GrapplePoint grapplePoint;
    private new ConstantForce constantForce;

    public Rigidbody Rigidbody => rigidbody;

    public Rigidbody AnchoredTo { get; private set; }
    public bool IsAnchored => AnchoredTo != null;

    private LineRenderer tether;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);

        grapplePoint = GetComponent<GrapplePoint>();
        Debug.Assert(grapplePoint != null);

        constantForce = GetComponent<ConstantForce>();
        Debug.Assert(constantForce != null);

        tether = GetComponentInChildren<LineRenderer>();
        Debug.Assert(tether != null);
    }

    void OnEnable()
    {
        if (spring != null)
        {
            spring.connectedBody = AnchoredTo;
        }
    }

    void Start()
    {
        if (IsAnchored == false)
        {
            DetermineFloatPath();
        }
    }

    void Update()
    {
        if (IsAnchored)
        {
            tether.SetPosition(1, transform.InverseTransformPoint(spring.connectedBody.transform.position));
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
            Debug.Assert(IsAnchored == false);
            Debug.Assert(spring == null);
            AnchoredTo = connectedRigidbody;
            spring = gameObject.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedBody = connectedRigidbody;
            spring.connectedAnchor = Vector3.zero;
            spring.maxDistance = springMaxDist;
            spring.connectedMassScale = 0.00001f;

            constantForce.enabled = true;

            grapplePoint.disabled = true;
        }
        else
        {
            Debug.Assert(IsAnchored == true);
            Debug.Assert(spring != null);
            AnchoredTo = null;
            Destroy(spring);
            spring = null;

            constantForce.enabled = false;

            grapplePoint.disabled = false;
        }
    }
}
