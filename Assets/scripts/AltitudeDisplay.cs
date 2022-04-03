using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;


public class AltitudeDisplay : MonoBehaviour
{
    [SerializeField] private Transform raftHeight; 
    [SerializeField] private Transform playerHeight; 
    [SerializeField] private Transform player; 
    [SerializeField] private Transform raft; 

    public float minHeight = 0;
    public float maxHeight = 500;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHeight.localScale = new Vector3(1, Mathf.Clamp(player.position.y / maxHeight, 0, 1), 1);
        raftHeight.localScale = new Vector3(1, Mathf.Clamp(raft.position.y / maxHeight, 0, 1), 1);
    }
}
