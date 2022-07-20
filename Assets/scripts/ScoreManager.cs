using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public event Action<int> OnScoreChanged;
    public event Action<int, int> OnCrateGoalReached;

    public int goldenCrateBonus = 3;

    public int crateGoal = 1;
    public IntScriptableObject raftMaxHeight;

    private List<GameObject> crates = new List<GameObject>();

    public int NumCrates =>
        crates.Count +
        crates.Count(x => x.CompareTag("GoldenCrate")) * goldenCrateBonus;
    
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

            var numCrates = NumCrates;

            OnScoreChanged?.Invoke(numCrates);

            if (numCrates >= crateGoal)
            {
                OnCrateGoalReached?.Invoke(numCrates, crateGoal);
            }
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.gameObject.CompareTag("Crate") || other.gameObject.CompareTag("GoldenCrate")){
            var idx = crates.IndexOf(other.gameObject);
            if (idx >= 0)
            {
                crates.RemoveAt(idx);
                OnScoreChanged?.Invoke(NumCrates);
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
