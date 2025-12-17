using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Board Cells (берётся из FieldsHolder)")]
    public Transform[] fields;

    [Header("Move Settings")]
    public float stepDelay = 0.3f;
    public float moveSpeed = 6f;

    [Header("State")]
    public int currentIndex = 0;
    public bool isMoving = false;

    [Header("Visual")]
    public float yOffset = 0.5f;

    private Vector3 cellOffset = Vector3.zero;

    private DiceRollScript dice;
    private Animator animator;

    private const string WALK_PARAM = "walk";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        dice = FindFirstObjectByType<DiceRollScript>();

        if (fields == null || fields.Length == 0)
        {
            FieldsHolder holder = FindFirstObjectByType<FieldsHolder>();
            if (holder != null && holder.fields != null && holder.fields.Length > 0)
                fields = holder.fields;
            else
                Debug.LogError("FieldsHolder not found OR FieldsHolder.fields is empty!");
        }

        MoveInstant(currentIndex);

        CellManager.Instance?.UpdateCell(currentIndex, this);
        SetWalking(false);
    }

    public void ApplyCellOffset(Vector3 offset)
    {
        cellOffset = offset;
        UpdatePositionWithOffset();
    }

    private void UpdatePositionWithOffset()
    {
        if (fields == null || fields.Length == 0) return;

        transform.position =
            fields[currentIndex].position +
            Vector3.up * yOffset +
            cellOffset;
    }

    private void SetWalking(bool walking)
    {
        if (animator == null) return;
        animator.SetBool(WALK_PARAM, walking);
    }

    public void MoveInstant(int index)
    {
        if (fields == null || fields.Length == 0) return;

        currentIndex = Mathf.Clamp(index, 0, fields.Length - 1);
        UpdatePositionWithOffset();
    }

    public void MoveBySteps(int steps)
    {
        if (isMoving) return;
        if (fields == null || fields.Length == 0) return;

        steps = Mathf.Max(0, steps);
        StartCoroutine(MoveStepsCoroutine(steps));
    }

    private IEnumerator MoveStepsCoroutine(int steps)
    {
        isMoving = true;
        SetWalking(true);

        int lastIndex = fields.Length - 1;

        int wanted = currentIndex + steps;
        int overshoot = 0;
        if (wanted > lastIndex)
        {
            overshoot = wanted - lastIndex; 
            wanted = lastIndex;             
        }

        while (currentIndex < wanted)
        {
            currentIndex++;
            yield return MoveToCurrentCell();
        }

        while (overshoot > 0 && currentIndex > 0)
        {
            currentIndex--;
            overshoot--;
            yield return MoveToCurrentCell();
        }

        CellManager.Instance?.UpdateCell(currentIndex, this);

        SetWalking(false);
        isMoving = false;

        if (dice == null) dice = FindFirstObjectByType<DiceRollScript>();
        dice?.NotifyMoveFinished();
    }

    private IEnumerator MoveToCurrentCell()
{
    Vector3 target = fields[currentIndex].position + Vector3.up * yOffset;

    while (Vector3.Distance(transform.position, target) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
        yield return null;
    }

    transform.position = target;
    yield return new WaitForSeconds(stepDelay);

    CellManager.Instance?.UpdateCell(currentIndex, this);
}
}
