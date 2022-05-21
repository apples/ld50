using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialMode : ChallengeMode
{
    public float seconds;

    public float TimeRemaining { get; private set; }

    private ChallengeGameManager manager;

    void Awake()
    {
        manager = GetComponent<ChallengeGameManager>();
        Debug.Assert(manager != null);
    }

    void Start()
    {
        TimeRemaining = seconds;
        manager.OpenExit();
    }

    protected override void ChallengeUpdate()
    {
        TimeRemaining = Mathf.Max(TimeRemaining - Time.deltaTime, 0f);

        var seconds = (int)TimeRemaining;
        var milliseconds = (int)((TimeRemaining - seconds) * 1000);

        manager.challengeText.text = "Find the Exit!";
        manager.timeRemainingText.text = $"{seconds:D2}:{milliseconds:D3}\n";

        if (TimeRemaining <= 0)
        {
            manager.Fail();
        }
    }
}
