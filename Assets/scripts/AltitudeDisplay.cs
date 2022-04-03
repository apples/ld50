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
    private float totalHeightScale;


    // Start is called before the first frame update
    void Start()
    {
        totalHeightScale = maxHeight - minHeight;
    }

    // Update is called once per frame
    void Update()
    {
        DisplayPlayerAndRaftHeight();
        RaiseMaxHeight();
    }

    private void DisplayPlayerAndRaftHeight(){
        float playerHeightScale = (player.position.y - minHeight) / maxHeight;
        playerHeight.localScale = new Vector3(1, Mathf.Clamp(playerHeightScale, 0, 1), 1);

        float raftHeightScale = (raft.position.y - minHeight) / maxHeight;
        raftHeight.localScale = new Vector3(1, Mathf.Clamp(raftHeightScale, 0, 1), 1);
    }

    private void RaiseMaxHeight(){
        if(player.position.y > maxHeight){
            maxHeight = player.position.y + 100;
        }

        if(raft.position.y > maxHeight){
            maxHeight = raft.position.y + 100;
        }
    }
}
