using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeGameManager : MonoBehaviour
{
    public InactiveSceneStack inactiveSceneStack;
    public ExitFlagsVariable exitFlags;

    public VoidEvent onFail;
    public VoidEvent onSuccess;

    public float endingDuration = 4f;

    public TMP_Text challengeText;

    private ChallengeMode challengeMode;

    private bool isEnding;
    private bool hasSucceeded;
    private float endTimer;
    private bool hasEnded;

    void Awake()
    {
        challengeMode = GetComponent<ChallengeMode>();
        Debug.Assert(challengeMode != null);
    }

    void Start()
    {
        Debug.Assert(inactiveSceneStack != null);
        Debug.Assert(onFail != null);
        Debug.Assert(onSuccess != null);
        Debug.Assert(challengeText != null);
    }

    void Update()
    {
        if (isEnding && !hasEnded)
        {
            endTimer -= Time.deltaTime;
            if (endTimer <= 0)
            {
                hasEnded = true;
                exitFlags.Value = hasSucceeded ? ExitFlags.Success : ExitFlags.None;

                Debug.Log($"Exiting challenge scene with ExitFlags = {exitFlags.Value}");

                var pendingUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                pendingUnload.completed += (_) =>
                {
                    var scene = inactiveSceneStack.Pop();
                    Debug.Log($"Making scene \"{scene.name}\" active");
                    SceneManager.SetActiveScene(scene);
                };
            }
        }
    }

    public void Fail()
    {
        Debug.Log($"Challenge failed");
        isEnding = true;
        endTimer = endingDuration;
        onFail.Raise();
        challengeMode.Stop();
    }

    public void Succeed()
    {
        Debug.Log($"Challenge succeeded");
        isEnding = true;
        endTimer = endingDuration;
        hasSucceeded = true;
        onSuccess.Raise();
        challengeMode.Stop();
    }
}
