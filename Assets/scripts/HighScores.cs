using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HighScores : MonoBehaviour
{
    [SerializeField] private string highScoreFileName;
    [SerializeField] private Transform scoreEntriesContainer;
    [SerializeField] private Transform scoreEntryTemplate;
    [SerializeField] private IntScriptableObject newScore;

    private List<ScoreEntry> scoreEntriesList;
    private List<Transform> scoreEntriesTransformList;

    private void Awake() {
        scoreEntryTemplate.gameObject.SetActive(false);
        AddScoreEntryToSavedScoreEntries(newScore.value);
        scoreEntriesList = LoadSavedScoreEntries();

        CreateScoreEntriesTransform();
    }

    private List<ScoreEntry> LoadSavedScoreEntries(){
        SerializableScoreEntries scoreEntries = JsonUtility.FromJson<SerializableScoreEntries>(PlayerPrefs.GetString(highScoreFileName));
        List<ScoreEntry> scoreEntriesList;
        if(scoreEntries != null){
            scoreEntriesList = scoreEntries.scoreEntryList;
        }else{
            scoreEntriesList = new List<ScoreEntry>();
        }
        SortScoreEntries(scoreEntriesList);
        return scoreEntriesList;
    }

    private void SaveScoreEntries(List<ScoreEntry> scoreEntriesList){
        string json = JsonUtility.ToJson(new SerializableScoreEntries {scoreEntryList = scoreEntriesList});
        PlayerPrefs.SetString(highScoreFileName, json);
        PlayerPrefs.Save();
    }
    private void CreateScoreEntriesTransform(){
        scoreEntriesTransformList = new List<Transform>();
        foreach(ScoreEntry scoreEntry in scoreEntriesList){
            CreateScoreEntryTransform(scoreEntry, scoreEntriesContainer, scoreEntriesTransformList);
        }
    }

    private void AddScoreEntryToSavedScoreEntries(int score){
        ScoreEntry scoreEntry = new ScoreEntry{ score = score, date = GetCurrentDisplayDate() };

        List<ScoreEntry> scoreEntries = LoadSavedScoreEntries();

        scoreEntries.Add(scoreEntry);
        SortScoreEntries(scoreEntries);

        if(scoreEntries.Count > 10){
            scoreEntries.RemoveRange(10, scoreEntries.Count - 10);
        }

        SaveScoreEntries(scoreEntries);
    }

    private void CreateScoreEntryTransform(ScoreEntry scoreEntry, Transform scoreEntriesContainer, List<Transform> scoreEntriesTransformList){
        float templateHeight = 33f;
        
        Transform scoreEntryTransform = Instantiate(scoreEntryTemplate, scoreEntriesContainer);
        RectTransform scoreEntryRectTransform = scoreEntryTransform.GetComponent<RectTransform>();
        scoreEntryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * scoreEntriesTransformList.Count);
        scoreEntryTransform.gameObject.SetActive(true);

        scoreEntryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = GetDisplayRankString(scoreEntriesTransformList.Count);
        scoreEntryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = scoreEntry.score.ToString();
        scoreEntryTransform.Find("Date").GetComponent<TextMeshProUGUI>().text = scoreEntry.date;

        scoreEntriesTransformList.Add(scoreEntryTransform);
    }

    // this class is just a convenience for JsonUtility to serialize
    private class SerializableScoreEntries{
        public List<ScoreEntry> scoreEntryList;
    }

    [System.Serializable]
    private class ScoreEntry{
        public int score;
        public string date;
    }

    private void SortScoreEntries(List<ScoreEntry> scoreEntriesList){
        for(int i = 0; i < scoreEntriesList.Count; i++){
            for(int j = i + 1; j < scoreEntriesList.Count; j++){
                if(scoreEntriesList[j].score > scoreEntriesList[i].score){
                    ScoreEntry tmp = scoreEntriesList[i];
                    scoreEntriesList[i] = scoreEntriesList[j];
                    scoreEntriesList[j] = tmp;
                }
            }
        }
    }

    private string GetDisplayRankString(int rank){
        rank = rank + 1; // display rank should not be 0 indexed
        switch(rank){
            default: return rank + "TH";
            case 1: return "1ST";
            case 2: return "2ND";
            case 3: return "3RD";
        }
    }

    private string GetCurrentDisplayDate(){
        DateTime now = DateTime.Now;
        return now.ToString("M/d/yyyy");
    }
}
