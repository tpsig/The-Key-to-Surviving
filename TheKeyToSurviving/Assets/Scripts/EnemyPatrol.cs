using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum MovementType
    {
        Horizontal,
        Vertical
    }

    [Header("Movement Type")]
    public MovementType movementType = MovementType.Horizontal;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float patrolDistance = 1f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (movementType == MovementType.Horizontal)
        {
            MoveHorizontal();
        }
        else
        {
            MoveVertical();
        }
    }

    void MoveHorizontal()
    {
        transform.position += new Vector3(
            direction * moveSpeed * Time.deltaTime,
            0,
            0
        );

        float distance = Vector3.Distance(startPosition, transform.position);

        if (distance > patrolDistance)
        {
            direction *= -1;
        }
    }

    void MoveVertical()
    {
        transform.position += new Vector3(
            0,
            direction * moveSpeed * Time.deltaTime,
            0
        );

        float distance = Vector3.Distance(startPosition, transform.position);

        if (distance > patrolDistance)
        {
            direction *= -1;
        }
    }
}