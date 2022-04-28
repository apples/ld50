using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public bool lockYaw;
    public bool lockPitch;
    public bool lockRoll;

    void LateUpdate()
    {
        var lookat = Camera.main.transform.position;

        var quat = Quaternion.LookRotation(transform.position - lookat, Vector3.up);

        var targetEuler = quat.eulerAngles;
        var currentEuler = transform.eulerAngles;

        if (lockYaw) targetEuler.y = currentEuler.y;
        if (lockPitch) targetEuler.x = currentEuler.x;
        if (lockRoll) targetEuler.z = currentEuler.z;

        quat.eulerAngles = targetEuler;

        transform.rotation = quat;
    }
}
