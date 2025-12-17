using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Links")]
    public DiceRollScript dice;

    [Header("UI")]
    public WinPanelUI winPanel; 

    private readonly List<PlayerMove> players = new();
    private int currentPlayerIndex = 0;
    private bool turnInProgress = false;
    private bool gameOver = false;

    void Start()
    {
        if (MatchStats.Instance != null && !Application.isEditor)
            MatchStats.Instance.StartMatch();
    }
    void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayersInOrder(List<PlayerMove> orderedPlayers)
    {
        players.Clear();
        players.AddRange(orderedPlayers);

        currentPlayerIndex = 0;
        gameOver = false;

        MatchStats.Instance?.StartMatch();

        StartTurn();
    }

    private void StartTurn()
    {
        if (gameOver) return;
        if (players.Count == 0) return;

        turnInProgress = true;

        PlayerMove p = players[currentPlayerIndex];
        var controller = p.GetComponent<PlayerController>();

        dice.SetCurrentPlayer(p);

        dice.allowHumanInput = controller != null && controller.isHuman;

        dice.ResetDice();

        if (!dice.allowHumanInput)
        {
            StartCoroutine(AIRollRoutine());
        }
    }

    private IEnumerator AIRollRoutine()
    {
        yield return new WaitForSeconds(0.6f);
        if (!gameOver) dice.RollDiceByCode();
    }

    public void EndTurn()
    {
        if (gameOver) return;
        if (!turnInProgress) return;

        turnInProgress = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        StartTurn();
    }

    public void GameOver(PlayerMove winner)
    {
        if (gameOver) return;
        gameOver = true;

        MatchStats.Instance?.StopMatch();

        string nickname = "Unknown";
        NameScript ns = winner.GetComponent<NameScript>();
        if (ns != null && !string.IsNullOrEmpty(ns.PlayerName))
            nickname = ns.PlayerName;

        float time = MatchStats.Instance != null ? MatchStats.Instance.GetElapsedSeconds() : 0f;
        int rolls = MatchStats.Instance != null ? MatchStats.Instance.GetRolls(nickname) : 0;

        Debug.Log($"Игра окончена! Победил: {nickname}, Time={time}, Rolls={rolls}");

        dice.allowHumanInput = false;

        if (winPanel != null)
        {
            winPanel.Show(nickname, time, rolls);
        }
        else
        {
            Debug.LogError("WinPanelUI не назначен в TurnManager!");
        }
    }
}
