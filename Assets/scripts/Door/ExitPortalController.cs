using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortalController : MonoBehaviour
{
    public VoidEvent onTouched;

    private bool hasBeenUsed;

    public void WarpPlayer()
    {
        if (hasBeenUsed) return;
        hasBeenUsed = true;

        onTouched.Raise();
    }
}
