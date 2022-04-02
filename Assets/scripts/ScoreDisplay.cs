using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI  textMeshPro;


    // Start is called before the first frame update
    void Start()
    {
        scoreManager.OnScoreChanged += ScoreDisplay_OnScoreChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ScoreDisplay_OnScoreChanged(object sender, ScoreManager.OnScoreChangedEventArgs e){
        textMeshPro.text = "Score: " + e.score.ToString();
    }
}
