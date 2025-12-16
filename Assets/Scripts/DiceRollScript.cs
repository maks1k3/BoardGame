using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    private Rigidbody rBody;
    private Vector3 startPosition;

    [Header("Forces")]
    [SerializeField] private float maxRandForcVal = 10f;
    [SerializeField] private float startRollingForce = 1200f;

    private float forceX, forceY, forceZ;

    [Header("Dice State")]
    public string diceFaceNum;         // "1".."6"
    public bool isLanded = false;      // приземлилс€ и стоит
    public bool firstThrow = false;    // был бросок в этом ходу

    [Header("Turn / Input")]
    public bool allowHumanInput = true;

    [Header("Current Player")]
    public PlayerMove playerMover;     // кто ходит сейчас

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ResetDice();
    }

    // TurnManager вызывает это в начале хода
    public void SetCurrentPlayer(PlayerMove p)
    {
        playerMover = p;
    }

    void Update()
    {
        if (playerMover == null) return;

        //  лик по кубику Ч только если это ход человека
        if (allowHumanInput && Input.GetMouseButtonDown(0) && (isLanded || !firstThrow))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    RollDice();
                    firstThrow = true;
                }
            }
        }

        //  огда кубик остановилс€ и бросок был Ч двигаем игрока
        if (isLanded && firstThrow)
        {
            if (int.TryParse(diceFaceNum, out int steps))
            {
                if (!playerMover.isMoving)
                {
                    playerMover.MoveBySteps(steps);
                    firstThrow = false; // чтобы второй раз не запускать движение
                }
            }
        }
    }

    // Ѕросок из кода (дл€ ботов)
    public void RollDiceByCode()
    {
        // чтобы бот не мог бросать, пока кубик ещЄ в движении
        if (!firstThrow && (isLanded || rBody.isKinematic))
        {
            RollDice();
            firstThrow = true;
        }
    }

    private void RollDice()
    {
        isLanded = false;

        rBody.isKinematic = false;

        forceX = Random.Range(0, maxRandForcVal);
        forceY = Random.Range(0, maxRandForcVal);
        forceZ = Random.Range(0, maxRandForcVal);

        rBody.AddForce(Vector3.up * Random.Range(800, startRollingForce));
        rBody.AddTorque(forceX, forceY, forceZ);
    }

    public void ResetDice()
    {
        rBody.isKinematic = true;
        transform.position = startPosition;
        transform.rotation = Random.rotation;

        isLanded = false;
        firstThrow = false;
    }

    /// <summary>
    /// ¬ызови этот метод ѕќ—Ћ≈ того, как PlayerMove закончил движение (в конце корутины).
    /// “ут решаем: конец игры или следующий ход.
    /// </summary>
    public void NotifyMoveFinished()
    {
        if (playerMover == null)
        {
            TurnManager.Instance.EndTurn();
            return;
        }

        // ѕроверка победы: выигрыш только если ќ—“јЌќ¬»Ћ—я на последней клетке
        if (playerMover.fields != null && playerMover.fields.Length > 0)
        {
            int lastIndex = playerMover.fields.Length - 1;
            if (playerMover.currentIndex == lastIndex)
            {
                TurnManager.Instance.GameOver(playerMover);
                return;
            }
        }

        TurnManager.Instance.EndTurn();
    }
}
