using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance { get; private set; }

    [SerializeField]
    private string saveFileName;

    private string FullFilePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private PersistentData cached;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public PersistentData Data
    {
        get
        {
            if (cached != null) return cached;

            var filePath = FullFilePath;

            Debug.Log($"Loading save data from: {filePath}");

            if (!File.Exists(filePath))
            {
                Debug.Log("Save file not found. Creating empty data.");
                cached = new PersistentData();

                var preexistingPlayerLevel = PlayerPrefs.GetInt("PlayerLevel", -1);
                if (preexistingPlayerLevel != -1)
                {
                    cached.playerLevel = preexistingPlayerLevel;
                }

                return cached;
            }

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open);
                using var reader = new StreamReader(stream);

                var fileContents = reader.ReadToEnd();

                Debug.Log($"Loaded save file contents: {fileContents}");

                cached = JsonUtility.FromJson<PersistentData>(fileContents);

                Debug.Log($"Load successful.");
                return cached;
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception occurred while loading data: {e}");
                cached = null;
                return cached;
            }
        }
    }

    public void WriteCachedData()
    {
        if (cached == null)
        {
            Debug.Log("Nothing to save.");
            return;
        }

        var filePath = FullFilePath;

        Debug.Log($"Saving save data to: {filePath}");

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var jsonData = JsonUtility.ToJson(cached, true);

            Debug.Log($"Writing json data: {jsonData}");

            using var stream = new FileStream(filePath, FileMode.Create);
            using var writer = new StreamWriter(stream);

            writer.Write(jsonData);

            Debug.Log($"Save successful.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception occurred while saving data: {e}");
        }
    }
}

[Serializable]
public class PersistentData
{
    public int playerLevel = 0;
    public List<ScoreEntry> highScores = new List<ScoreEntry>(10);

    [System.Serializable]
    public class ScoreEntry
    {
        public int score;
        
        // local
        public string date;
        
        // online
        public string persona;
        public int globalRank;
    }
}
