using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoldenBalloonsMode : ChallengeMode
{
    public float seconds;

    public List<Balloon> balloons;

    public float TimeRemaining { get; private set; }

    public int BalloonsFound => balloons.Count(x => x.IsAnchored);

    public int NumBalloons => balloons.Count;

    private ChallengeGameManager manager;

    void Awake()
    {
        manager = GetComponent<ChallengeGameManager>();
        Debug.Assert(manager != null);
    }

    void Start()
    {
        TimeRemaining = seconds;
    }

    protected override void ChallengeUpdate()
    {
        TimeRemaining = Mathf.Max(TimeRemaining - Time.deltaTime, 0f);

        var seconds = (int)TimeRemaining;
        var milliseconds = (int)((TimeRemaining - seconds) * 1000);

        manager.challengeText.text =
            $"Find the balloons:\n" +
            $"{BalloonsFound} / {NumBalloons}\n";
        manager.timeRemainingText.text = $"{seconds:D2}:{milliseconds:D3}\n";
        
        if (BalloonsFound == NumBalloons)
        {
            manager.OpenExit();
        }
        else if (TimeRemaining <= 0)
        {
            manager.Fail();
        }
    }
}
