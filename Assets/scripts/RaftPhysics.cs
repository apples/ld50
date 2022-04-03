using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftPhysics : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private List<AnchorPoint> anchorPoints = new();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Debug.Assert(rigidbody != null);

        foreach (Transform anchorPoint in transform.Find("AnchorPoints"))
        {
            anchorPoints.Add(anchorPoint.GetComponent<AnchorPoint>());
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        foreach (var anchorPoint in anchorPoints)
        {
            var bias = (transform.position.y - transform.TransformPoint(anchorPoint.transform.position).y + 1) * 1;
            var upwardForce = anchorPoint.UpwardForce * bias;
            
            rigidbody.AddForceAtPosition(new Vector3(0, upwardForce, 0), anchorPoint.transform.position, ForceMode.Force);
        }
    }
}
