using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public static CellManager Instance;

    private Dictionary<int, List<PlayerMove>> playersOnCell = new();

    [Header("Offset")]
    public float offsetRadius = 0.35f;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateCell(int cellIndex, PlayerMove player)
    {
        foreach (var list in playersOnCell.Values)
            list.Remove(player);

        if (!playersOnCell.ContainsKey(cellIndex))
            playersOnCell[cellIndex] = new List<PlayerMove>();

        playersOnCell[cellIndex].Add(player);

        ApplyOffsets(cellIndex);
    }

    private void ApplyOffsets(int cellIndex)
    {
        var list = playersOnCell[cellIndex];
        int count = list.Count;

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * offsetRadius;

            list[i].ApplyCellOffset(offset);
        }
    }
}
