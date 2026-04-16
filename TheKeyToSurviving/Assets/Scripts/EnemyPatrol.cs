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

    [Header("Base Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float basePatrolDistance = 2f;

    [Header("Random Variation")]
    public float speedRandomRange = 1.5f;
    public float distanceRandomRange = 1f;

    private float moveSpeed;
    private float patrolDistance;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool goingPositive = true;

    public override void OnNetworkSpawn()
    {
        startPosition = transform.position;

        if (IsServer)
        {
            RandomizeStats();
            SetNextTarget();
        }
    }

    void Update()
    {
        if (!IsServer) return;

        PatrolMove();
    }

    void RandomizeStats()
    {
        moveSpeed = baseMoveSpeed + Random.Range(-speedRandomRange, speedRandomRange);
        patrolDistance = basePatrolDistance + Random.Range(-distanceRandomRange, distanceRandomRange);

        // safety clamps (prevents weird values)
        moveSpeed = Mathf.Clamp(moveSpeed, 1f, 20f);
        patrolDistance = Mathf.Clamp(patrolDistance, 0.5f, 10f);
    }

    void PatrolMove()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

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