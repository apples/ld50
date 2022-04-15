using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeGameManager : MonoBehaviour
{
    public InactiveSceneStack inactiveSceneStack;

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

                var pendingUnload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                pendingUnload.completed += (_) =>
                {
                    var scene = inactiveSceneStack.Pop();
                    SceneManager.SetActiveScene(scene);
                };
            }
        }
    }

    public void Fail()
    {
        isEnding = true;
        endTimer = endingDuration;
        onFail.Raise();
        challengeMode.Stop();
    }

    public void Succeed()
    {
        isEnding = true;
        endTimer = endingDuration;
        hasSucceeded = true;
        onSuccess.Raise();
        challengeMode.Stop();
    }
}
