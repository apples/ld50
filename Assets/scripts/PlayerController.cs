using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    public GameObject cameraBoom;
    public float sensitivity = 1;
    public float speed = 1;
    public float jumpSpeed;

    public float legRideHeight;
    public float legRayDistance;
    public float legSpringConstant;
    public float legDamping;

    public float groundMovementDamping;

    public float throwForce;

    private new Rigidbody rigidbody;

    private Vector2 movementInput;
    private Vector2 aimInput;
    private bool jumpInput;
    private bool isOnGround;
    private Rigidbody groundRigidbody;
    private bool isJumping;

    private Rigidbody heldItem;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Debug.Assert(cameraBoom != null);
    }

    void Update()
    {
        // If we don't have focus and you click in the game, capture the mouse
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        // Don't update if we don't have focus
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        // Press Escapse to release mouse control
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        var jumpPressed = Input.GetKeyDown(KeyCode.Space);

        movementInput += new Vector2(h, v) * speed * Time.deltaTime;
        aimInput += new Vector2(mouseX, mouseY) * sensitivity * Time.deltaTime;
        if (jumpPressed) jumpInput = true;

        // pick up / throw
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
            {
                var colliders = Physics.OverlapSphere(transform.position, 1f);

                foreach (var collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Crate"))
                    {
                        heldItem = collider.attachedRigidbody;
                        heldItem.transform.SetParent(transform);
                        heldItem.transform.localPosition = new Vector3(0, 1.5f, 0);
                        heldItem.detectCollisions = false;
                        heldItem.isKinematic = true;
                        break;
                    }
                }
            }
            else
            {
                var forward = cameraBoom.transform.forward;
                var up = cameraBoom.transform.up;
                var throwDir = (forward + up * 0.75f).normalized;

                heldItem.transform.SetParent(null);
                heldItem.isKinematic = false;
                heldItem.detectCollisions = true;
                heldItem.AddForce(throwDir * throwForce, ForceMode.Impulse);

                heldItem = null;
            }
        }
    }

    void FixedUpdate()
    {
        ProcessLegs();

        // Only process input if we have focus
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            ProcessMouseInput();
            ProcessMovementInput();

            movementInput = default;
            aimInput = default;
            jumpInput = false;
        }
    }

    private void ProcessLegs()
    {
        var rayDir = Vector3.down;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), rayDir, out RaycastHit hitInfo, legRayDistance + 0.1f, ~Physics.IgnoreRaycastLayer))
        {
            var compressionDistance = (hitInfo.distance - 0.1f) - legRideHeight;

            var selfVel = Vector3.Dot(rayDir, rigidbody.velocity);
            var groundVel = hitInfo.rigidbody != null ? Vector3.Dot(rayDir, hitInfo.rigidbody.velocity) : 0f;
            var relVel = selfVel - groundVel;

            var springForce = compressionDistance * legSpringConstant - relVel * legDamping;

            // only apply downward force if not jumping, but always apply upward force
            if (springForce < 0 || !isJumping)
            {
                rigidbody.AddForce(rayDir * springForce);
            }

            // reset isJumping if we're travelling downwards towards ground
            if (springForce < 0)
            {
                isJumping = false;
            }

            // apply ground movement damping
            if (!isJumping)
            {
                var selfLateralVel = rigidbody.velocity - selfVel * rayDir;
                var groundLateralVel = hitInfo.rigidbody != null ? hitInfo.rigidbody.velocity - groundVel * rayDir : Vector3.zero;

                var relLateralVel = groundLateralVel - selfLateralVel;

                rigidbody.velocity += relLateralVel * groundMovementDamping;
            }

            isOnGround = true;
            groundRigidbody = hitInfo.rigidbody;
        }
        else
        {
            isOnGround = false;
            groundRigidbody = null;
        }
    }

    private void ProcessMovementInput()
    {
        // move

        transform.position +=
            transform.forward * movementInput.y +
            transform.right * movementInput.x;

        // jump

        //float j = Input.GetAxisRaw("Jump");
        if (jumpInput && isOnGround && !isJumping)
        {
            var vel = rigidbody.velocity;
            vel.y =
                jumpSpeed +
                (groundRigidbody != null ? groundRigidbody.velocity.y : 0);
            rigidbody.velocity = vel;
            isJumping = true;
        }
    }

    private void ProcessMouseInput()
    {
        // rotate player left-right

        var rotation = transform.localEulerAngles;
        rotation.y += aimInput.x;
        transform.localEulerAngles = rotation;

        // rotate camera boom up-down

        var camRotation = cameraBoom.transform.localEulerAngles;
        camRotation.x -= aimInput.y;
        if (camRotation.x < 0) camRotation.x += 360;
        if (camRotation.x <= 180 && camRotation.x > 90) camRotation.x = 90;
        if (camRotation.x > 180 && camRotation.x < 270) camRotation.x = 270;
        cameraBoom.transform.localEulerAngles = camRotation;
    }

    // If the game loses focus, unlock the cursor
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
