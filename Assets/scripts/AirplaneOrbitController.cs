using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneOrbitController : MonoBehaviour
{
    public Vector3 worldUp = Vector3.up;
    public float speed = 1f;

    public Rigidbody airplane;

    private Vector3 Normal => Vector3.Cross(airplane.transform.localPosition, Vector3.Cross(worldUp, airplane.transform.localPosition)).normalized;

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // position

        var arcLength = speed * Time.deltaTime;
        
        var radius = Vector3.Magnitude(airplane.transform.localPosition);

        var angleDeltaRadians = arcLength / radius;

        var normal = Normal;

        var quat = Quaternion.AngleAxis(Mathf.Rad2Deg * angleDeltaRadians, normal);

        var newLocalPos = quat * airplane.transform.localPosition;

        airplane.MovePosition(transform.position + newLocalPos);

        // orientation

        var toAirplane = newLocalPos.normalized;

        var newAirplaneForward = Vector3.Cross(normal, toAirplane);

        if (speed < 0)
        {
            newAirplaneForward = -newAirplaneForward;
        }

        var lookatUp = (normal + -toAirplane).normalized;

        airplane.MoveRotation(Quaternion.LookRotation(newAirplaneForward, lookatUp));
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireCube(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        if (airplane != null)
        {
            UnityEditor.Handles.DrawWireDisc(transform.position, Normal, Vector3.Distance(transform.position, airplane.transform.position));
            UnityEditor.Handles.DrawLine(transform.position, transform.position + Normal);
        }
    }
#endif

}
