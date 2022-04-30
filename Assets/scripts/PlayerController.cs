using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]

    public GameObject cameraBoom;
    public new Camera camera;

    public float cameraSmoothing = 42f;

    [Header("Sprite")]
    
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Effects")]

    public GameObject tetherGroup;
    public GameObject crosshair;

    [Header("Audio")]

    public AudioMixer audioMixer;
    public AudioSource sfxBalloon;
    public AudioSource sfxZoop;
    public AudioSource sfxJump;
    public AudioSource sfxGrab;
    public AudioSource sfxScream;

    [Header("Input")] 
    private const string MOUSE_SENSITIVITY_KEYWORD = "mouseAimSensitivity";
    private const string CONTROLLER_SENSITIVITY_KEYWORD = "controllerAimSensitivity";
    public float mouseSensitivity;
    public float controllerSensitivity;
    public string inputDevice = "";

    [Header("Physics")]

    public new Collider collider;

    [Header("Movement")]

    public float moveSpeed = 1;
    public float moveAcceleration;
    public float airAccelerationFactor;

    public float hardSpeedLimit;
    public float softSpeedLimit;
    public float softSpeedLimitDamping;

    [Header("Legs")]

    public float legRideHeight;
    public float legRayDistance;
    public float legSpringConstant;
    public float legDamping;

    public float groundMovementDamping;

    public LayerMask legsRaycastLayers;

    [Header("Jump")]

    public float jumpSpeed;
    public float spinJumpExtraSpeed;
    public float comboJumpTimerMax;
    public float attachedHandMaxJumpDist;

    public float diveSpeed;
    public float diveMinVerticalVelocity;

    public int upwardMovementIgnoresLayer;

    [Header("Ground Pound")] 
    public float groundPoundSpeed;

    [Header("Hand")]

    public GameObject handPrefab;
    public float handLaunchSpeed;
    public float maxHandDist;
    public float handGrappleForce;
    public float minHandGrappleDistance;
    public float itemThrowForce;

    [Header("Pause")]

    public PauseMenu pauseMenu;

    // private fields

    private new Rigidbody rigidbody;
    private PlayerInputActions playerInputActions;
    private Vector2 movementInput;
    private Vector2 aimInput;
    private PlayerInput playerInput;
    private bool jumpInput;
    private bool groundPoundInput;
    private int groundPoundCharges = 1;
    private bool isOnGround;
    private float coyoteTime;
    private int coyoteCharges = 1;
    private Rigidbody groundRigidbody;
    private bool isJumping;
    private bool isDiving;

    private Rigidbody heldItem;

    private HandController thrownHand;

    private List<Balloon> balloons = new List<Balloon>();

    private int paramFacingX = Animator.StringToHash("FacingX");
    private int paramFacingY = Animator.StringToHash("FacingY");
    private int paramIsWalking = Animator.StringToHash("IsWalking");
    private int paramIsSpinning = Animator.StringToHash("IsSpinning");
    private int paramIsDiving = Animator.StringToHash("IsDiving");
    private int paramIsHolding = Animator.StringToHash("IsHolding");

    private Vector3 facingDir = Vector3.forward;
    private bool isWalking;
    private bool isSpinning;
    private float comboJumpTimer;

    private bool IsDivingOrGrappling => isDiving || thrownHand != null && thrownHand.IsAttached && !thrownHand.IsPulling;
    private bool isCrosshairValid = false;

    private bool disableMovement;

    private Vector3? fixedCameraPosition;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);

        mouseSensitivity = PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEYWORD, 10);
        controllerSensitivity = PlayerPrefs.GetFloat(CONTROLLER_SENSITIVITY_KEYWORD, 10);
    }

    void Start()
    {
        playerInputActions = InputManager.inputActions;
        playerInputActions.Player.Enable();
        
        Debug.Assert(cameraBoom != null);
        Debug.Assert(camera != null);
        Debug.Assert(animator != null);
        Debug.Assert(spriteRenderer != null);
        Debug.Assert(handPrefab != null);
        Debug.Assert(collider != null);
        Debug.Assert(tetherGroup != null);

        Cursor.lockState = CursorLockMode.None;

        tetherGroup.SetActive(false);

        UpdateCrosshair(true);
    }

    void Update()
    {
        if(pauseMenu.IsGamePaused)
        {
            return;
        }

        // If we don't have focus and you click in the game, capture the mouse
        if (Cursor.lockState == CursorLockMode.None && playerInputActions.Player.Grapple.WasPerformedThisFrame())
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
        if (playerInputActions.Player.Escape.WasPerformedThisFrame())
        {
            Cursor.lockState = CursorLockMode.None;
        }

        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        float h = inputVector.x;
        float v = inputVector.y;
        Vector2 aimVector = playerInputActions.Player.Aim.ReadValue<Vector2>();
        var jumpPressed = playerInputActions.Player.Jump.WasPerformedThisFrame();
        var interactPressed = playerInputActions.Player.Interact.WasPerformedThisFrame();
        var groundPoundPressed = playerInputActions.Player.GroundPound.WasPerformedThisFrame();

        GetInputDevice();
        
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            movementInput = new Vector2(h, v);
            float inputSensitivity;
            if (inputDevice == "Mouse") inputSensitivity = mouseSensitivity;
            else inputSensitivity = controllerSensitivity * 10;

            // var inputSensitivity = inputDevice == "Mouse" ? sensitivity : sensitivity * 10;
            aimInput += aimVector * (inputSensitivity * Time.deltaTime);
            if (jumpPressed) jumpInput = true;
            if (groundPoundPressed)
            {
                groundPoundInput = true;
                Debug.Log("groundPoundPressed");
            }
        }

        //grapple
        if (playerInputActions.Player.Grapple.WasPerformedThisFrame())
        {
            ThrowHand();
        }

        // tie balloon
        if (playerInputActions.Player.TieDown.WasPerformedThisFrame())
        {
            TryTieBalloon();
        }

        //grab/throw
        if (interactPressed)
        {
            if (heldItem == null)
            {
                if (TryGrabCrate())
                {
                    if (thrownHand != null)
                    {
                        DestroyHand();
                        return;
                    }
                }
            }
            else
            {
                ThrowHeldItem();
            }
        }

        // update animator

        if (!disableMovement)
        {
            var animatorFacing = Quaternion.Inverse(transform.rotation) * facingDir;

            animator.SetFloat(paramFacingX, -animatorFacing.x);
            animator.SetFloat(paramFacingY, animatorFacing.z);
            animator.SetBool(paramIsWalking, isWalking);
            animator.SetBool(paramIsSpinning, isSpinning);
            animator.SetBool(paramIsHolding, heldItem != null);
            animator.SetBool(paramIsDiving, IsDivingOrGrappling);

            spriteRenderer.flipX = -animatorFacing.x > 0 && -animatorFacing.x > animatorFacing.z && !isSpinning;
        }
        else
        {
            animator.SetBool(paramIsWalking, false);
            animator.SetBool(paramIsSpinning, true);
            animator.SetBool(paramIsDiving, false);
            spriteRenderer.flipX = false;
        }

        // combo jump timer
        comboJumpTimer -= Time.deltaTime;
        if (comboJumpTimer < 0) comboJumpTimer = 0;

        //crosshair color

        UpdateCrosshair();

        // update tether

        if (thrownHand != null && thrownHand.IsAttached)
        {
            if (!tetherGroup.activeSelf)
            {
                tetherGroup.SetActive(true);
                sfxZoop.Play();
            }

            var tetherRange = tetherGroup.transform.childCount + 1;
            var currentTether = 0;
            foreach (Transform tether in tetherGroup.transform)
            {
                ++currentTether;
                tether.SetPositionAndRotation(
                    Vector3.Lerp(transform.position, thrownHand.transform.position, (float)currentTether / (float)tetherRange),
                    Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up)
                );
            }
        }
        else
        {
            tetherGroup.SetActive(false);
        }
    }

    private void UpdateCrosshair(bool force = false)
    {
        if (CursorRaycast(out RaycastHit hitInfo))
        {
            if (!isCrosshairValid || force)
            {
                isCrosshairValid = true;
                crosshair.GetComponent<Renderer>().material.color = Color.green;
            }
        }
        else
        {
            if (isCrosshairValid || force)
            {
                isCrosshairValid = false;
                crosshair.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    void FixedUpdate()
    {
        ProcessLegs();
        ProcessMouseInput();
        ProcessMovementInput();

        aimInput = default;
        jumpInput = false;
        groundPoundInput = false;

        // grapple to hand
        if (thrownHand != null && thrownHand.IsAttached && !thrownHand.IsAttachedTo.isPulled)
        {
            var toHand = thrownHand.transform.position - transform.position;

            if (toHand.magnitude >= minHandGrappleDistance)
            {
                rigidbody.AddForce(toHand.normalized * handGrappleForce);
            }

            isSpinning = false;
        }

        if (rigidbody.velocity.magnitude > softSpeedLimit)
        {
            var m = rigidbody.velocity.magnitude;
            var d = m - softSpeedLimit;
            m = softSpeedLimit + d * softSpeedLimitDamping;
            rigidbody.velocity = rigidbody.velocity.normalized * m;
        }

        if (rigidbody.velocity.magnitude > hardSpeedLimit)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * hardSpeedLimit;
        }

        Physics.IgnoreLayerCollision(gameObject.layer, upwardMovementIgnoresLayer, rigidbody.velocity.y > 0);

        if (fixedCameraPosition is Vector3 pos)
        {
            var rot = Quaternion.LookRotation((transform.position - pos).normalized, Vector3.up);

            rot = Quaternion.Slerp(camera.transform.rotation, rot, 1f - Mathf.Exp(-cameraSmoothing * Time.fixedDeltaTime));

            camera.transform.position = pos;
            camera.transform.rotation = rot;
        }
    }

    private void ThrowHand()
    {
        if (thrownHand != null)
        {
            DestroyHand();
            return;
        }

        var cameraRaycastDist = maxHandDist - cameraBoom.transform.localPosition.z;

        var target = camera.transform.position + camera.transform.forward * cameraRaycastDist;
        
        if (CursorRaycast(out var hitInfo))
        {
            target = hitInfo.point;
        }

        var hand = Instantiate(handPrefab);
        hand.transform.position = cameraBoom.transform.position;
        hand.transform.rotation = Quaternion.LookRotation((target - hand.transform.position).normalized, Vector3.up);

        var handBody = hand.GetComponent<Rigidbody>();
        handBody.velocity = hand.transform.forward * handLaunchSpeed;

        thrownHand = hand.GetComponent<HandController>();
        thrownHand.audioMixer = audioMixer;
        Debug.Assert(thrownHand != null);
        thrownHand.playerController = this;

        sfxGrab.Play();
    }

    private bool CursorRaycast(out RaycastHit hitInfo)
    {
        return Physics.Raycast(cameraBoom.transform.position, camera.transform.forward, out hitInfo, maxHandDist, ~Physics.IgnoreRaycastLayer);
    }

    public void DestroyHand()
    {
        Debug.Assert(thrownHand != null);

        if (thrownHand.IsAttached && !thrownHand.IsPulling)
        {
            isDiving = false;
        }

        thrownHand.ReleaseHeldItem();

        Destroy(thrownHand.gameObject);
        thrownHand = null;
    }

    private void ThrowHeldItem()
    {
        var forward = cameraBoom.transform.forward;
        var up = cameraBoom.transform.up;
        var throwDir = (forward + up * 0.75f).normalized;

        heldItem.transform.SetParent(null);
        heldItem.isKinematic = false;
        heldItem.detectCollisions = true;
        heldItem.AddForce(throwDir * itemThrowForce * heldItem.mass, ForceMode.Impulse);

        heldItem = null;
    }

    private bool TryGrabCrate()
    {
        var colliders = Physics.OverlapSphere(transform.position, 1f);

        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Crate"))
            {
                GrabCrate(collider.attachedRigidbody);
                return true;
            }
        }

        return false;
    }

    private bool TryTieBalloon()
    {
        if (balloons.Count == 0) return false;

        var colliders = Physics.OverlapSphere(transform.position - new Vector3(0, 0.5f, 0), 1f);

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<AnchorPoint>() is AnchorPoint anchorPoint)
            {
                var balloon = balloons[balloons.Count - 1];
                balloon.AnchorTo(null);
                anchorPoint.GiveBalloon(balloon.gameObject);
                balloons.RemoveAt(balloons.Count - 1);
                sfxBalloon.Play();
                return true;
            }
        }

        return false;
    }

    private void GrabCrate(Rigidbody crateRigidbody)
    {
        Debug.Assert(crateRigidbody.gameObject.CompareTag("Crate"));
        heldItem = crateRigidbody;
        heldItem.transform.SetParent(transform);
        heldItem.transform.localPosition = new Vector3(0, 1.5f, 0);
        heldItem.detectCollisions = false;
        heldItem.isKinematic = true;

        crateRigidbody.GetComponent<LayerObject>().PreventDespawn = true;

        sfxGrab.Play();
    }
    
    private void GetInputDevice()
    {
        if (playerInputActions.Player.Aim.activeControl != null)
        {
            inputDevice = playerInputActions.Player.Aim.activeControl.device.name;
        }
    }

    private void ProcessLegs()
    {
        var rayDir = Vector3.down;

        if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), rayDir, out RaycastHit hitInfo, legRayDistance + 0.1f, legsRaycastLayers))
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
            if (springForce < 0 && isJumping)
            {
                isJumping = false;
                isSpinning = false;
                isDiving = false;
                comboJumpTimer = comboJumpTimerMax;
            }

            isOnGround = true;
            coyoteTime = 0;
            coyoteCharges = 1;
            groundRigidbody = hitInfo.rigidbody;
            groundPoundCharges = 1;
        }
        else
        {
            isOnGround = false;
            isJumping = true;
            coyoteTime += Time.fixedDeltaTime;
            groundRigidbody = null;
        }
    }

    private void ProcessMovementInput()
    {
        if (disableMovement) return;

        // move

        var localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
        var velocity = new Vector2(localVelocity.x, localVelocity.z);

        var speedClampedVelocity = velocity.normalized * Mathf.Min(velocity.magnitude, moveSpeed);

        var desiredVelocity = movementInput.normalized * moveSpeed;

        var toDesired = desiredVelocity - speedClampedVelocity;

        var accelVector = Mathf.Min(toDesired.magnitude, moveAcceleration * Time.fixedDeltaTime) * toDesired.normalized;

        if (!isOnGround)
        {
            if (movementInput == Vector2.zero && IsDivingOrGrappling)
            {
                accelVector = Vector2.zero;
            }
            else
            {
                accelVector /= airAccelerationFactor;
            }
        }

        velocity = velocity + accelVector;

        rigidbody.velocity = transform.TransformDirection(
            new Vector3(
                velocity.x,
                localVelocity.y,
                velocity.y
            )
        );

        // set animation parameters

        if (movementInput != Vector2.zero)
        {
            var worldMovementInput = transform.rotation * new Vector3(movementInput.x, 0, movementInput.y).normalized;
            facingDir = worldMovementInput;
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if (IsDivingOrGrappling)
        {
            facingDir = Vector3.Scale(rigidbody.velocity, new Vector3(1, 0, 1)).normalized;
        }

        // jump and dive

        if (jumpInput)
        {
            var canJump = coyoteTime < .5 && coyoteCharges > 0;

            if (thrownHand != null && thrownHand.IsAttached && (thrownHand.transform.position - transform.position).magnitude < attachedHandMaxJumpDist)
            {
                canJump = true;
                DestroyHand();
            }

            if (canJump)
            {
                isDiving = false;
                coyoteCharges--;
                var vel = rigidbody.velocity;
                vel.y =
                    jumpSpeed +
                    (groundRigidbody != null ? groundRigidbody.velocity.y : 0);
                isJumping = true;

                if (comboJumpTimer > 0)
                {
                    comboJumpTimer = 0;
                    isSpinning = true;
                    vel.y += spinJumpExtraSpeed;
                }

                rigidbody.velocity = vel;

                sfxJump.Play();
            }
            else if (!isOnGround && !IsDivingOrGrappling)
            {
                var vel = rigidbody.velocity;
                vel += transform.forward * diveSpeed;
                if (vel.y < diveMinVerticalVelocity)
                {
                    vel.y = diveMinVerticalVelocity;
                }
                rigidbody.velocity = vel;
                isDiving = true;
                isSpinning = false;

                sfxJump.Play();
            }
        }

        if (groundPoundInput && !isOnGround && groundPoundCharges > 0)
        {
            rigidbody.velocity = transform.up * (-1 * groundPoundSpeed);
            isDiving = false;
            isSpinning = false;
            
            
            // ground pound todo - sfx here
        }
    }

    private void ProcessMouseInput()
    {
        if (fixedCameraPosition != null) return;

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

    public void GiveItem(GrapplePoint isAttachedTo)
    {
        if (isAttachedTo.gameObject.CompareTag("Crate"))
        {
            if (heldItem == null)
            {
                GrabCrate(isAttachedTo.GetComponent<Rigidbody>());
            }
        }
        else if (isAttachedTo.GetComponent<Balloon>() is Balloon balloon)
        {
            balloons.Add(balloon);
            balloon.AnchorTo(rigidbody);
            sfxBalloon.Play();
            balloon.GetComponent<LayerObject>().PreventDespawn = true;
        }
    }

    public void Fail()
    {
        disableMovement = true;
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
    }

    public void Succeed()
    {
        disableMovement = true;
        rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
    }

    public void Fall()
    {
        sfxScream.Play();
        fixedCameraPosition = camera.transform.position;
        disableMovement = true;
    }
}
