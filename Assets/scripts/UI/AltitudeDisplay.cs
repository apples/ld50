using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AltitudeDisplay : MonoBehaviour
{
    [SerializeField] private Transform raftHeight; 
    [SerializeField] private Transform playerHeight; 
    [SerializeField] private Transform player; 
    [SerializeField] private Transform raft; 

    public float minHeight = 0;
    public float maxHeight = 500;

    void Update()
    {
        DisplayPlayerAndRaftHeight();
        // RaiseMaxHeight();
        AdjustMinAndMaxHeight();
    }

    private void DisplayPlayerAndRaftHeight()
    {
        float heightDifference = maxHeight - minHeight;
        if (player != null)
        {
            float playerHeightScale = (player.position.y - minHeight) / heightDifference;
            playerHeight.localScale = new Vector3(1, Mathf.Clamp(playerHeightScale, 0, 1), 1);
        }

        if (raft != null)
        {
            float raftHeightScale = (raft.position.y - minHeight) / heightDifference;
            raftHeight.localScale = new Vector3(1, Mathf.Clamp(raftHeightScale, 0, 1), 1);
        }
    }

    private void AdjustMinAndMaxHeight()
    {
        minHeight = Mathf.Max(Mathf.Min(player.position.y, raft.position.y) - 25, 0);
        maxHeight = Mathf.Max(player.position.y, raft.position.y) + 25;
    }

    private void RaiseMaxHeight(){
        if (player != null)
        {
            if (player.position.y > maxHeight)
            {
                maxHeight = player.position.y + 100;
            }
        }

        if (raft != null)
        {
            if (raft.position.y > maxHeight)
            {
                maxHeight = raft.position.y + 100;
            }
        }
    }
}
