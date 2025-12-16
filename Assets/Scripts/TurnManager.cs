using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Links")]
    public DiceRollScript dice;

    private readonly List<PlayerMove> players = new();
    private int currentPlayerIndex = 0;
    private bool turnInProgress = false;
    private bool gameOver = false;



    void Awake()
    {
        Instance = this;
    }

    // Регистрируем игроков в нужном порядке
    public void RegisterPlayersInOrder(List<PlayerMove> orderedPlayers)
    {
        players.Clear();
        players.AddRange(orderedPlayers);

        currentPlayerIndex = 0;
        StartTurn();
    }

    private void StartTurn()
    {
        if (players.Count == 0) return;

        turnInProgress = true;

        PlayerMove p = players[currentPlayerIndex];
        var controller = p.GetComponent<PlayerController>();

        dice.SetCurrentPlayer(p);

        // человеку разрешаем кликать по кубику, ботам - нет
        dice.allowHumanInput = controller != null && controller.isHuman;

        // сброс кубика перед ходом (по желанию)
        dice.ResetDice();

        // если бот — пусть сам кинет
        if (!dice.allowHumanInput)
        {
            StartCoroutine(AIRollRoutine());
        }
    }

    private IEnumerator AIRollRoutine()
    {
        yield return new WaitForSeconds(0.6f);
        dice.RollDiceByCode(); // программный бросок
    }

    // вызывается, когда игрок закончил двигаться
    public void EndTurn()
    {
        if (!turnInProgress) return;

        turnInProgress = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        StartTurn();
    }
    public void GameOver(PlayerMove winner)
    {
        if (gameOver) return;
        gameOver = true;

        Debug.Log("Игра окончена! Победил: " + winner.name);

        // на всякий случай запрещаем кидать кубик
        dice.allowHumanInput = false;
    }
}
