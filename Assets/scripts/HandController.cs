using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HandController : MonoBehaviour
{
    public AudioSource sfxGrab;
    public AudioMixer audioMixer;

    public float returnSpeed;
    public float returnTime;

    public PlayerController playerController;

    public GrapplePoint IsAttachedTo { get; private set; }
    public bool IsAttached => IsAttachedTo != null;

    public bool IsPulling => IsAttached && IsAttachedTo.isPulled;

    private new Rigidbody rigidbody;
    private Vector3 startingPosition;
    private bool returning;

    private List<string> itemTags = new List<string>{"Crate", "Balloon"};
    public GameObjectVariable itemsContainer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        Debug.Assert(playerController != null);
        Debug.Assert(itemsContainer != null);

        startingPosition = transform.position;

        sfxGrab.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
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
                var item = IsAttachedTo;
                ReleaseHeldItem();
                playerController.GiveItem(item);
            }

            playerController.DestroyHand();
        }
    }

    public void ReleaseHeldItem()
    {
        if (IsAttached)
        {
            if (IsPulling)
            {
                if (itemsContainer.Value != null && itemTags.Contains(IsAttachedTo.tag))
                {
                    IsAttachedTo.transform.SetParent(itemsContainer.Value.transform);
                }
                else
                {
                    IsAttachedTo.transform.SetParent(null);
                }

                if (IsAttachedTo.GetComponent<Rigidbody>() is Rigidbody grappleRigidbody)
                {
                    grappleRigidbody.detectCollisions = true;
                    grappleRigidbody.isKinematic = false;
                }
            }

            IsAttachedTo = null;
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

        if (collider.isTrigger) return;

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

        IsAttachedTo = grapplePoint;
        rigidbody.velocity = Vector3.zero;
        rigidbody.detectCollisions = false;

        sfxGrab.Play();
    }
}
