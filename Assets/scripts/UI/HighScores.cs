using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static PersistentData;
using Steamworks;
using UnityEngine.Serialization;


public class HighScores : MonoBehaviour
{
    [FormerlySerializedAs("scoreEntriesContainer")] [SerializeField] private Transform localScoreEntriesContainer;
    [FormerlySerializedAs("scoreEntryTemplate")] [SerializeField] private Transform localScoreEntryTemplate;    
    [SerializeField] private Transform globalScoreEntriesContainer;
    [SerializeField] private Transform globalScoreEntryTemplate;
    [SerializeField] private Transform unableToLoadContainer;
    [SerializeField] private IntScriptableObject newScore;

    [SerializeField] private bool showLevelInfo = true;
    
    public string scoreMetricText = "";

    private List<ScoreEntry> localScoreEntriesList;
    private List<ScoreEntry> globalScoreEntriesList;
    private List<Transform> localScoreEntriesTransformList;
    private List<Transform> globalScoreEntriesTransformList;
    private int level;
    private int currentTarget;

    private SteamLeaderboardController steamLeaderboardController;
    private String allTimeHighScoreLeaderboardName = "AllTimeHighScores";

    private List<string> unlocks = new List<string>(){
        "Rainbows",
        "Golden Doors",
        //"a new challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "a new challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "a new challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "a new balloon challenge room",
        "a new hell challenge room",
        "more Rainbows!",
    };

    private void Start()
    {
        steamLeaderboardController = GetComponent<SteamLeaderboardController>();
        
        if (showLevelInfo)
        {
            //do levelups first
            level = PersistentDataManager.Instance.Data.playerLevel;
            if (level == -1)
            {
                level = 0;
            }
            int oldLevel = level;
            currentTarget = level * 50;

            if (newScore.value > currentTarget)
            {
                level++;
                if (!LoadLocalSavedScoreEntries().Exists(x => x.score > newScore.value))
                {
                    level++;
                    GameObject.Find("Level Summary").GetComponent<TextMeshProUGUI>().text = "You hit your target height and got a new personal best!";
                }
                else
                {
                    GameObject.Find("Level Summary").GetComponent<TextMeshProUGUI>().text = "You hit your target height!";
                }
            }
            else if (!LoadLocalSavedScoreEntries().Exists(x => x.score > newScore.value))
            {
                level++;
                GameObject.Find("Level Summary").GetComponent<TextMeshProUGUI>().text = "You got a new personal best!";
            }
            else
            {
                GameObject.Find("Level Summary").GetComponent<TextMeshProUGUI>().text = "";
            }

            GameObject.Find("Current Level").GetComponent<TextMeshProUGUI>().text = "Current Level: " + level;

            currentTarget = level * 50;
            GameObject.Find("Current Target").GetComponent<TextMeshProUGUI>().text = "Next Target: " + currentTarget + scoreMetricText;

            //get rewards every even level up to level 34
            if (oldLevel != level && (level % 2 == 0 || level - oldLevel == 2) && level < unlocks.Count + 1)
            {
                GameObject.Find("Level Rewards").GetComponent<TextMeshProUGUI>().text = "You've unlocked " + unlocks[(level / 2) - 1] + "!";
            }
            else if (level < unlocks.Count + 1)
            {
                GameObject.Find("Level Rewards").GetComponent<TextMeshProUGUI>().text = "New unlock in " + (level % 2 == 0 ? "2 levels" : "1 level");
            }

            PersistentDataManager.Instance.Data.playerLevel = level;
            PersistentDataManager.Instance.WriteCachedData();
        }
        
        AddScoreEntryToSavedScoreEntries(newScore.value);
        localScoreEntryTemplate.gameObject.SetActive(false);
        globalScoreEntryTemplate.gameObject.SetActive(false);
        
        localScoreEntriesList = LoadLocalSavedScoreEntries();
        CreateLocalScoreEntriesTransform();
    }

    public void LoadGlobalScoreEntries()
    {
        unableToLoadContainer.gameObject.SetActive(false);
        globalScoreEntriesList = steamLeaderboardController.ScoreEntries;
        CreateGlobalScoreEntriesTransform();
    }

    private List<ScoreEntry> LoadLocalSavedScoreEntries()
    {
        localScoreEntriesList = new List<ScoreEntry>(PersistentDataManager.Instance.Data.highScores);
        SortScoreEntries(localScoreEntriesList);
        return localScoreEntriesList;
    }

    private void SaveScoreEntries(List<ScoreEntry> scoreEntriesList)
    {
        PersistentDataManager.Instance.Data.highScores = scoreEntriesList;
        PersistentDataManager.Instance.WriteCachedData();
    }

    private void CreateLocalScoreEntriesTransform()
    {
        localScoreEntriesTransformList = new List<Transform>();
        foreach (ScoreEntry scoreEntry in localScoreEntriesList)
        {
            CreateLocalScoreEntryTransform(scoreEntry, localScoreEntriesContainer, localScoreEntriesTransformList);
        }
    }
    
    private void CreateGlobalScoreEntriesTransform()
    {
        globalScoreEntriesTransformList = new List<Transform>();
        foreach (ScoreEntry scoreEntry in globalScoreEntriesList)
        {
            CreateGlobalScoreEntryTransform(scoreEntry, globalScoreEntriesContainer, globalScoreEntriesTransformList);
        }
    }

    private void AddScoreEntryToSavedScoreEntries(int score)
    {
        if (score == 0)
        {
            return;
        }
        
        ScoreEntry scoreEntry = new ScoreEntry{ score = score, date = GetCurrentDisplayDate() };

        List<ScoreEntry> scoreEntries = LoadLocalSavedScoreEntries();

        scoreEntries.Add(scoreEntry);
        SortScoreEntries(scoreEntries);

        if(scoreEntries.Count > 10){
            scoreEntries.RemoveRange(10, scoreEntries.Count - 10);
        }

        SaveScoreEntries(scoreEntries);
        steamLeaderboardController.UploadScoreToSteamLeaderboard(allTimeHighScoreLeaderboardName, score);
    }

    private void CreateLocalScoreEntryTransform(ScoreEntry scoreEntry, Transform scoreEntriesContainer, List<Transform> scoreEntriesTransformList)
    {
        float templateHeight = 33f;
        
        Transform scoreEntryTransform = Instantiate(localScoreEntryTemplate, scoreEntriesContainer);
        RectTransform scoreEntryRectTransform = scoreEntryTransform.GetComponent<RectTransform>();
        scoreEntryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * scoreEntriesTransformList.Count);
        scoreEntryTransform.gameObject.SetActive(true);

        scoreEntryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = GetDisplayRankString(scoreEntriesTransformList.Count + 1);
        scoreEntryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = scoreEntry.score.ToString() + " " + scoreMetricText;
        scoreEntryTransform.Find("Date").GetComponent<TextMeshProUGUI>().text = scoreEntry.date;

        scoreEntriesTransformList.Add(scoreEntryTransform);
    }
    
    private void CreateGlobalScoreEntryTransform(ScoreEntry scoreEntry, Transform scoreEntriesContainer, List<Transform> scoreEntriesTransformList)
    {
        float templateHeight = 33f;
        
        Transform scoreEntryTransform = Instantiate(globalScoreEntryTemplate, scoreEntriesContainer);
        RectTransform scoreEntryRectTransform = scoreEntryTransform.GetComponent<RectTransform>();
        scoreEntryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * scoreEntriesTransformList.Count);
        scoreEntryTransform.gameObject.SetActive(true);

        scoreEntryTransform.Find("Position").GetComponent<TextMeshProUGUI>().text = GetDisplayRankString(scoreEntry.globalRank);
        scoreEntryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = scoreEntry.score + " " + scoreMetricText;
        scoreEntryTransform.Find("User").GetComponent<TextMeshProUGUI>().text = scoreEntry.persona;

        scoreEntriesTransformList.Add(scoreEntryTransform);
    }

    private void SortScoreEntries(List<ScoreEntry> scoreEntriesList)
    {
        scoreEntriesList.Sort((a, b) => b.score.CompareTo(a.score));
    }

    private string GetDisplayRankString(int rank){
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
