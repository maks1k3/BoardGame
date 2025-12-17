using System.Collections.Generic;
using UnityEngine;

public class MatchStats : MonoBehaviour
{
    public static MatchStats Instance;

    private float startRealtime;
    private float endRealtime;
    private bool running;

    private Dictionary<string, int> rollsByPlayer = new();

    private void Awake()
    {
        Instance = this;
    }

    public void StartMatch()
    {
        rollsByPlayer.Clear();
        startRealtime = Time.realtimeSinceStartup;
        endRealtime = startRealtime;
        running = true;
    }

    public float GetElapsedSeconds()
    {
        float now = running ? Time.realtimeSinceStartup : endRealtime;
        return now - startRealtime;
    }

    public void StopMatch()
    {
        endRealtime = Time.realtimeSinceStartup;
        running = false;
    }

    public void AddRoll(string playerName)
    {
        if (!rollsByPlayer.ContainsKey(playerName))
            rollsByPlayer[playerName] = 0;

        rollsByPlayer[playerName]++;
    }

    public int GetRolls(string playerName)
    {
        return rollsByPlayer.TryGetValue(playerName, out int v) ? v : 0;
    }
}
