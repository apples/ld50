using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortalController : MonoBehaviour
{
    public VoidEvent onTouched;

    public GameObject playerTrigger;

    public Animator animator;

    private bool hasBeenUsed;

    private int isOpenParam = Animator.StringToHash("IsOpen");

    void Start()
    {
        playerTrigger.SetActive(false);
        animator.SetBool(isOpenParam, false);
    }

    public void Open()
    {
        playerTrigger.SetActive(true);
        animator.SetBool(isOpenParam, true);
    }

    public void WarpPlayer()
    {
        if (hasBeenUsed) return;
        hasBeenUsed = true;

        onTouched.Raise();
    }
}
