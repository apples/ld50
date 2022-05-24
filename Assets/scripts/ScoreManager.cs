using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event EventHandler<OnCratesChangedEventArgs> OnScoreChanged;
    public class OnCratesChangedEventArgs : EventArgs{
        public int numCrates;
    }

    public event EventHandler<OnCrateGoalReachedEventArgs> OnCrateGoalReached;
    public class OnCrateGoalReachedEventArgs : EventArgs
    {
    }

    public int crateGoal = 1;
    public IntScriptableObject raftMaxHeight;

    private List<GameObject> crates = new List<GameObject>();

    public int NumCrates => crates.Count + crates.FindAll(x => x.CompareTag("GoldenCrate")).Count;
    public IReadOnlyList<GameObject> Crates => crates;

    private void Update() {
        UpdateMaxRaftHeight();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Crate") || other.gameObject.CompareTag("GoldenCrate")){
            Debug.Assert(crates.IndexOf(other.gameObject) == -1);
            crates.Add(other.gameObject);

            if (other.gameObject.GetComponent<LayerObject>() is LayerObject layerObject)
            {
                layerObject.PreventDespawn = true;
            }

            OnScoreChanged?.Invoke(this, new OnCratesChangedEventArgs{numCrates = NumCrates});

            if (NumCrates >= crateGoal)
            {
                OnCrateGoalReached?.Invoke(this, new OnCrateGoalReachedEventArgs());
            }
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Crate") || other.gameObject.CompareTag("GoldenCrate")){
            var idx = crates.IndexOf(other.gameObject);
            if (idx >= 0)
            {
                crates.RemoveAt(idx);
                OnScoreChanged?.Invoke(this, new OnCratesChangedEventArgs { numCrates = NumCrates });
            }
        }
    }

    public void DestroyAllCrates()
    {
        foreach (var crate in crates)
        {
            Destroy(crate);
        }
        crates.Clear();
    }

    private void UpdateMaxRaftHeight(){
        if(raftMaxHeight == null){
            return;
        }
        
        int raftHeight = (int) Math.Round(gameObject.transform.position.y);
        if(raftHeight > raftMaxHeight.value){
            raftMaxHeight.value = raftHeight;
        }
    }
}
