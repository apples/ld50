using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float minFloatDirectionTime = 1f;
    public float maxFloatDirectionTime = 3f;
    private float floatDirectionTime;
    private float floatDirectionTimer;
    private Vector3 floatDirection;
    public float minFloatSpeed;
    public float maxFloatSpeed;
    private float floatSpeed;

    private GameObject raft;

    public bool isAnchored = false;
    public bool IsAnchored
    { 
        get => isAnchored;
        set
        {
            isAnchored = value;
            if (rigidbody != null && isAnchored == true)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }
    }

    private new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);

        if (isAnchored == false)
        {
            DetermineFloatPath();
        }
    }

    void Update()
    {
        if (isAnchored)
        {

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


}
