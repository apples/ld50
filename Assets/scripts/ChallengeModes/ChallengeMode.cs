using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChallengeMode : MonoBehaviour
{
    private bool stopped;

    void Update()
    {
        if (!stopped)
        {
            ChallengeUpdate();
        }
    }

    protected abstract void ChallengeUpdate();

    public void Stop()
    {
        stopped = true;
    }
}
