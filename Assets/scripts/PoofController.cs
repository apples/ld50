using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoofController : MonoBehaviour
{
    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = camera.transform.forward;
    }
}
