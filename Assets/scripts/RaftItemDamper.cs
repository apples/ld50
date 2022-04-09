using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftItemDamper : MonoBehaviour
{
    public float dampingFactor;
    private float debug_tiltFactor;

    void OnTriggerStay(Collider collider)
    {
        if (collider.attachedRigidbody != null && collider.attachedRigidbody.GetComponent<CrateBehavior>() is CrateBehavior crate)
        {
            var tiltFactor = Mathf.Clamp(1f - Vector3.Angle(Vector3.up, this.transform.up) / 45f, 0f, 1f);
            this.debug_tiltFactor = tiltFactor;

            var rigidbody = collider.attachedRigidbody;
            rigidbody.AddForce(-rigidbody.velocity * dampingFactor * tiltFactor, ForceMode.Acceleration);
        }
    }
}
