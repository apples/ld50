using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event EventHandler<OnScoreChangedEventArgs> OnScoreChanged;
    public class OnScoreChangedEventArgs : EventArgs{
        public int score;
    }

    public int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Crate")){
            score++;
            OnScoreChanged?.Invoke(this, new OnScoreChangedEventArgs{score = score});
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Crate")){
            score--;
            OnScoreChanged?.Invoke(this, new OnScoreChangedEventArgs{score = score});
        }
    }


}
