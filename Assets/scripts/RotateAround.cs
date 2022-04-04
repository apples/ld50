using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public GameObject target;
    public float rotateSpeed = -5;
    public float zOffset = 15;


    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, target.transform.position.y + zOffset, transform.position.z);
    }
}
