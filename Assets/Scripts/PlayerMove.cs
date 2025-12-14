using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Transform[] fields;   // 12 клеток по порядку
    public float stepDelay = 0.3f;
    public float moveSpeed = 6f;

    public int currentIndex = 0;
    public bool isMoving = false;

    void Start()
    {
        MoveInstant(0);
    }

    public void MoveBySteps(int steps)
    {
        if (!isMoving)
            StartCoroutine(MoveCoroutine(steps));
    }

    IEnumerator MoveCoroutine(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            currentIndex++;

            if (currentIndex >= fields.Length)
                currentIndex = fields.Length - 1;

            Vector3 target = fields[currentIndex].position + Vector3.up * 0.5f;

            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(stepDelay);
        }

        isMoving = false;
    }

    void MoveInstant(int index)
    {
        transform.position = fields[index].position + Vector3.up * 0.5f;
    }
}
