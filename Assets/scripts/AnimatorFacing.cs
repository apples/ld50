using UnityEngine;

public class AnimatorFacing : MonoBehaviour
{
    public Transform forwardTransform;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private int paramFacingX = Animator.StringToHash("FacingX");
    private int paramFacingY = Animator.StringToHash("FacingY");

    private Vector3 facingDir = Vector3.forward;

    void Start()
    {
        Debug.Assert(animator != null);
        Debug.Assert(spriteRenderer != null);

        if (forwardTransform == null)
        {
            forwardTransform = this.transform;
        }
    }

    void Update()
    {
        var animatorFacing = Quaternion.Inverse(Camera.main.transform.rotation) * forwardTransform.forward;

        animator.SetFloat(paramFacingX, -animatorFacing.x);
        animator.SetFloat(paramFacingY, animatorFacing.z);

        spriteRenderer.flipX = animatorFacing.x > 0 && animatorFacing.x > animatorFacing.z;
    }
}

