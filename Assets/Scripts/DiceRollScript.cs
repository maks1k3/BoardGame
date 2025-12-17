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
    public string diceFaceNum;         
    public bool isLanded = false;      
    public bool firstThrow = false;    

    [Header("Turn / Input")]
    public bool allowHumanInput = true;

    [Header("Current Player")]
    public PlayerMove playerMover;     

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ResetDice();
    }

    public void SetCurrentPlayer(PlayerMove p)
    {
        playerMover = p;
    }

    private string GetCurrentNickname()
    {
        if (playerMover == null) return "Unknown";

        NameScript ns = playerMover.GetComponent<NameScript>();
        if (ns != null && !string.IsNullOrEmpty(ns.PlayerName))
            return ns.PlayerName;

        return playerMover.gameObject.name;
    }

    void Update()
    {
        if (playerMover == null) return;

        if (allowHumanInput && Input.GetMouseButtonDown(0) && (isLanded || !firstThrow))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    RollDice();

                    MatchStats.Instance?.AddRoll(GetCurrentNickname());

                    firstThrow = true;
                }
            }
        }

        if (isLanded && firstThrow)
        {
            if (int.TryParse(diceFaceNum, out int steps))
            {
                if (!playerMover.isMoving)
                {
                    playerMover.MoveBySteps(steps);
                    firstThrow = false; 
                }
            }
        }
    }

    public void RollDiceByCode()
    {
        if (!firstThrow && (isLanded || rBody.isKinematic))
        {
            RollDice();

            MatchStats.Instance?.AddRoll(GetCurrentNickname());

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
    /// Вызови этот метод ПОСЛЕ того, как PlayerMove закончил движение (в конце корутины).
    /// Тут решаем: конец игры или следующий ход.
    /// </summary>
    public void NotifyMoveFinished()
    {
        if (playerMover == null)
        {
            TurnManager.Instance.EndTurn();
            return;
        }

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
