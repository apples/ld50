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
    public ExitFlagsVariable exitFlags;

    public Transform playerRespawnLocation;
    public Transform fairySpawnLocation;

    public GameObject fairyPrefab;

    private Animator animator;

    private int isOpenHash = Animator.StringToHash("IsOpen");

    private AsyncOperation pendingLoad;

    private bool hasBeenUsed;

    private GameObject player;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null);
    }

    void Start()
    {
        Debug.Assert(!String.IsNullOrEmpty(warpToScene));
        Debug.Assert(inactiveSceneStack != null);
        Debug.Assert(exitFlags != null);
        Debug.Assert(playerRespawnLocation != null);
        Debug.Assert(fairySpawnLocation != null);
        Debug.Assert(fairyPrefab != null);
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

    public void WarpPlayer(GameObject player)
    {
        if (pendingLoad != null || hasBeenUsed) return;

        Debug.Log($"Loading scene \"{warpToScene}\"");

        pendingLoad = SceneManager.LoadSceneAsync(warpToScene, LoadSceneMode.Additive);
        pendingLoad.allowSceneActivation = false;

        hasBeenUsed = true;

        this.player = player;
    }

    private void SwapScenes()
    {
        inactiveSceneStack.Push(SceneManager.GetActiveScene(), this.SceneManager_OnSceneResume);
        pendingLoad.allowSceneActivation = true;
        pendingLoad.completed += (_) =>
        {
            pendingLoad = null;
            var scene = SceneManager.GetSceneByName(warpToScene);
            Debug.Log($"Switching to challenge scene \"{scene.name}\"");
            SceneManager.SetActiveScene(scene);
        };
    }

    private void SceneManager_OnSceneResume()
    {
        Debug.Log($"Resuming Door's parent scene");
        player.transform.SetPositionAndRotation(playerRespawnLocation.position, playerRespawnLocation.rotation);

        if (exitFlags.Value.HasFlag(ExitFlags.Success))
        {
            Instantiate(fairyPrefab, fairySpawnLocation.position, fairySpawnLocation.rotation);
        }
    }
}
