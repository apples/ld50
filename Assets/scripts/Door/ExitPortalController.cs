using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPortalController : MonoBehaviour
{
    public InactiveSceneStack inactiveSceneStack;

    private AsyncOperation pendingUnload;

    private bool hasBeenUsed;

    void Start()
    {
        Debug.Assert(inactiveSceneStack != null);
    }

    void Update()
    {
    }

    public void WarpPlayer()
    {
        if (pendingUnload != null || hasBeenUsed) return;

        pendingUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        pendingUnload.completed += (_) =>
        {
            pendingUnload = null;
            SwapScenes();
        };

        hasBeenUsed = true;
    }

    private void SwapScenes()
    {
        var scene = inactiveSceneStack.Pop();
        SceneManager.SetActiveScene(scene);
    }
}
