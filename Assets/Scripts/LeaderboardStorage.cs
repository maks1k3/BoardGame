using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public float timeSeconds;
    public int rolls;
    public int score;       
    public string dateIso;  
}

[Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public static class LeaderboardStorage
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "leaderboard.json");

    public static LeaderboardData Load()
    {
        if (!File.Exists(FilePath))
            return new LeaderboardData();

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
            return new LeaderboardData();

        var data = JsonUtility.FromJson<LeaderboardData>(json);
        if (data == null || data.entries == null)
            return new LeaderboardData();

        return data;
    }

    public static void Save(LeaderboardData data)
    {
        if (data == null) data = new LeaderboardData();
        if (data.entries == null) data.entries = new List<LeaderboardEntry>();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    /// <summary>
    /// Добавить результат в leaderboard.json
    /// </summary>
    public static void AddEntry(string name, float timeSeconds, int rolls, int score)
    {
        var data = Load();

        data.entries.Add(new LeaderboardEntry
        {
            playerName = string.IsNullOrEmpty(name) ? "Unknown" : name,
            timeSeconds = Mathf.Max(0f, timeSeconds),
            rolls = Mathf.Max(0, rolls),
            score = Mathf.Max(0, score),
            dateIso = DateTime.UtcNow.ToString("o")
        });

        data.entries.Sort((a, b) =>
        {
            int s = b.score.CompareTo(a.score); 
            if (s != 0) return s;

            int t = a.timeSeconds.CompareTo(b.timeSeconds); 
            if (t != 0) return t;

            return a.rolls.CompareTo(b.rolls);
        });

        if (data.entries.Count > 50)
            data.entries.RemoveRange(50, data.entries.Count - 50);

        Save(data);

        Debug.Log("Saved leaderboard to: " + FilePath);
    }

    public static void Clear()
    {
        Save(new LeaderboardData());
    }
}
