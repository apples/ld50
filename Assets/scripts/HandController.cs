using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public AudioSource sfxGrab;

    public float returnSpeed;
    public float returnTime;

    public PlayerController playerController;

    public bool IsAttached { get; private set; }
    public GrapplePoint IsAttachedTo { get; private set; }

    public bool IsPulling => IsAttached && IsAttachedTo.isPulled;

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
            if (IsPulling)
            {
                IsAttachedTo.transform.SetParent(null);
                if (IsAttachedTo.GetComponent<Rigidbody>() is Rigidbody grappleRigidbody)
                {
                    grappleRigidbody.detectCollisions = true;
                    grappleRigidbody.isKinematic = false;
                }
                playerController.GiveItem(IsAttachedTo);
            }

            playerController.DestroyHand();
        }
    }

    void FixedUpdate()
    {
        if (returning)
        {
            var toPlayer = playerController.transform.position - transform.position;
            var dir = toPlayer.normalized;

            if (IsPulling)
            {
                transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            var speed = Mathf.Max(toPlayer.magnitude / returnTime, returnSpeed);

            rigidbody.velocity = dir * speed;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (returning) return;

        var grapplePoint = collider.GetComponent<GrapplePoint>();

        if (grapplePoint == null) return;

        if (grapplePoint.disabled) return;

        var attachPoint = collider.ClosestPoint(transform.position);

        transform.rotation = Quaternion.LookRotation((attachPoint - transform.position).normalized, Vector3.up);
        transform.position = attachPoint;

        if (!grapplePoint.isPulled)
        {
            transform.SetParent(collider.transform, worldPositionStays: true);
            rigidbody.isKinematic = true;
        }
        else
        {
            grapplePoint.transform.SetParent(transform, worldPositionStays: true);
            
            if (grapplePoint.GetComponent<Rigidbody>() is Rigidbody grappleRigidbody)
            {
                grappleRigidbody.detectCollisions = false;
                grappleRigidbody.isKinematic = true;
            }

            returning = true;
        }

        IsAttached = true;
        IsAttachedTo = grapplePoint;
        rigidbody.velocity = Vector3.zero;
        rigidbody.detectCollisions = false;

        sfxGrab.Play();
    }
}
