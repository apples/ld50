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

    private bool? isOpen;

    private int isOpenParam = Animator.StringToHash("IsOpen");

    public void Close()
    {
        if (isOpen != false)
        {
            playerTrigger.SetActive(false);
            animator.SetBool(isOpenParam, false);
            isOpen = false;
        }
    }

    public void Open()
    {
        if (isOpen != true)
        {
            Debug.Log($"Portal opened");
            playerTrigger.SetActive(true);
            animator.SetBool(isOpenParam, true);
            isOpen = true;
        }
    }

    public void WarpPlayer()
    {
        Debug.Log($"Warping player");

        if (hasBeenUsed) return;
        hasBeenUsed = true;

        onTouched.Raise();
    }
}
