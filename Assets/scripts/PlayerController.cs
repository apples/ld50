using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        //float j = Input.GetAxisRaw("Jump");

        gameObject.transform.position = new Vector3 (transform.position.x + (h * speed), gameObject.transform.position.y,
            transform.position.z + (v * speed));

        if(Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, new Vector3(0, -1, 0), 1.0f)){
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        }
        
    }
}
