using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public float returnSpeed;

    public PlayerController playerController;

    public bool IsAttached { get; private set; }

    private new Rigidbody rigidbody;
    private Vector3 startingPosition;
    private bool returning;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        Debug.Assert(playerController != null);
        startingPosition = transform.position;
    }

    void Update()
    {
        if ((transform.position - startingPosition).magnitude > playerController.maxHandDist)
        {
            returning = true;
            rigidbody.detectCollisions = false;
        }

        if (returning && (playerController.transform.position - transform.position).magnitude < 1f)
        {
            playerController.DestroyHand();
        }
    }

    void FixedUpdate()
    {
        if (returning)
        {
            var dir = (playerController.transform.position - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            rigidbody.velocity = dir * returnSpeed;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (returning) return;

        var grapplePoint = collider.GetComponent<GrapplePoint>();

        if (grapplePoint == null) return;

        var attachPoint = collider.ClosestPoint(transform.position);

        transform.rotation = Quaternion.LookRotation((attachPoint - transform.position).normalized, Vector3.up);
        transform.position = attachPoint;

        transform.SetParent(collider.transform, worldPositionStays: true);

        IsAttached = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        rigidbody.detectCollisions = false;
    }
}
