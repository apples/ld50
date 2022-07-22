using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public Vector3 localUp = Vector3.up;

    void LateUpdate()
    {
        var up = transform.parent != null ? transform.parent.TransformDirection(localUp) : localUp;

        var forward = Vector3.ProjectOnPlane(-Camera.main.transform.forward, up);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
