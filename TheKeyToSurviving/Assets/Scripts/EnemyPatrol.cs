using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour
{
    public enum MovementType
    {
        Horizontal,
        Vertical
    }

    [Header("Movement Type")]
    public MovementType movementType = MovementType.Horizontal;

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float patrolDistance = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool goingPositive = true;

    public override void OnNetworkSpawn()
    {
        startPosition = transform.position;

        if (!IsServer) return;

        SetNextTarget();
    }

    void Update()
    {
        if (!IsServer) return;

        PatrolMove();
    }

    void PatrolMove()
    {
        // Move toward target (smooth + stable)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // When we reach target, flip direction
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            goingPositive = !goingPositive;
            SetNextTarget();
        }
    }

    void SetNextTarget()
    {
        Vector3 dir =
            (movementType == MovementType.Horizontal)
                ? Vector3.right
                : Vector3.up;

        float sign = goingPositive ? 1f : -1f;

        targetPosition = startPosition + dir * patrolDistance * sign;
    }
}