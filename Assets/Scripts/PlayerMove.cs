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

    void Start()
    {
        // Автоподхват клеток из FieldsHolder (в сцене)
        if (fields == null || fields.Length == 0)
        {
            FieldsHolder holder = FindFirstObjectByType<FieldsHolder>();
            if (holder != null && holder.fields != null && holder.fields.Length > 0)
            {
                fields = holder.fields;
            }
            else
            {
                Debug.LogError("FieldsHolder not found OR FieldsHolder.fields is empty!");
                return;
            }
        }

        MoveInstant(currentIndex);
    }

    // Мгновенно поставить на клетку (например старт)
    public void MoveInstant(int index)
    {
        if (fields == null || fields.Length == 0) return;

        index = Mathf.Clamp(index, 0, fields.Length - 1);
        currentIndex = index;

        transform.position = fields[currentIndex].position + Vector3.up * 0.5f;
    }

    // Движение на N шагов (кубик)
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

        int lastIndex = fields.Length - 1;
        int dir = +1; //  +1 идём вперёд, -1 идём назад

        for (int i = 0; i < steps; i++)
        {
            // если стоим на финише и ещё есть шаги — начинаем идти назад
            if (currentIndex == lastIndex) dir = -1;

            int nextIndex = currentIndex + dir;

            // защита: если вдруг вышли за границы — разворачиваемся
            if (nextIndex > lastIndex)
            {
                dir = -1;
                nextIndex = currentIndex + dir;
            }
            else if (nextIndex < 0)
            {
                dir = +1;
                nextIndex = currentIndex + dir;
            }

            currentIndex = nextIndex;

            Vector3 target = fields[currentIndex].position + Vector3.up * 0.5f;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = target;
            yield return new WaitForSeconds(stepDelay);
        }

        isMoving = false;

        // после движения сообщаем менеджеру ходов
        FindFirstObjectByType<DiceRollScript>()?.NotifyMoveFinished();
    }
}
