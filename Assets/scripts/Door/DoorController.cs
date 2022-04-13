using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    public string warpToScene;
    public bool isOpen;

    public InactiveSceneStack inactiveSceneStack;

    private Animator animator;

    private int isOpenHash = Animator.StringToHash("IsOpen");

    private AsyncOperation pendingLoad;

    private bool hasBeenUsed;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null);
    }

    void Start()
    {
        Debug.Assert(!String.IsNullOrEmpty(warpToScene));
        Debug.Assert(inactiveSceneStack != null);
    }

    void Update()
    {
        animator.SetBool(isOpenHash, isOpen && !hasBeenUsed);

        if (pendingLoad != null)
        {
            if (pendingLoad.progress >= 0.9f)
            {
                SwapScenes();
            }
        }
    }

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }

    public void WarpPlayer()
    {
        if (pendingLoad != null || hasBeenUsed) return;

        pendingLoad = SceneManager.LoadSceneAsync(warpToScene, LoadSceneMode.Additive);
        pendingLoad.allowSceneActivation = false;

        hasBeenUsed = true;
    }

    private void SwapScenes()
    {
        inactiveSceneStack.Push(SceneManager.GetActiveScene());
        pendingLoad.allowSceneActivation = true;
        pendingLoad.completed += (_) =>
        {
            pendingLoad = null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(warpToScene));
        };
    }
}
