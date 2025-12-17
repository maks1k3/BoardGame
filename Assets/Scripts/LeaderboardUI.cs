using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LeaderboardUI : MonoBehaviour
{
    public Transform content;
    public GameObject entryPrefab;

    private const int MAX_ENTRIES = 7;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (content == null || entryPrefab == null)
        {
            
            return;
        }

        for (int i = content.childCount - 1; i >= 0; i--)
        {
            var child = content.GetChild(i).gameObject;
            if (child == entryPrefab) continue; 
            Destroy(child);
        }

        var data = LeaderboardStorage.Load();
        if (data == null || data.entries == null || data.entries.Count == 0)
            return;

        var top = data.entries
            .OrderByDescending(e => e.score)  
            .Take(MAX_ENTRIES)
            .ToList();

        for (int i = 0; i < top.Count; i++)
        {
            var e = top[i];

            GameObject go = Instantiate(entryPrefab, content);
            go.SetActive(true);

            Text t = go.GetComponent<Text>();
            if (t != null)
                t.text = $"{i + 1}. {e.playerName} — {e.score}";
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
