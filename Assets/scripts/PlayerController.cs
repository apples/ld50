using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{
    public GameObject cameraBoom;
    public float sensitivity = 1;
    public float speed = 1;

    private new Rigidbody rigidbody;

    private Vector2 movementInput;
    private Vector2 aimInput;
    private bool jumpInput;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);
    }

    void Start()
    {
        Debug.Assert(cameraBoom != null);
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        var jumpPressed = Input.GetKeyDown(KeyCode.Space);

        movementInput += new Vector2(h, v) * speed * Time.deltaTime;
        aimInput += new Vector2(mouseX, mouseY) * sensitivity * Time.deltaTime;
        if (jumpPressed) jumpInput = true;
    }

    void FixedUpdate()
    {
        ProcessMouseInput();
        ProcessMovementInput();

        movementInput = default;
        aimInput = default;
        jumpInput = false;
    }

    private void ProcessMovementInput()
    {
        // move

        transform.position +=
            transform.forward * movementInput.y +
            transform.right * movementInput.x;

        // jump

        //float j = Input.GetAxisRaw("Jump");
        if (jumpInput && Physics.Raycast(transform.position, new Vector3(0, -1, 0), 1.1f))
        {
            rigidbody.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
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
}
