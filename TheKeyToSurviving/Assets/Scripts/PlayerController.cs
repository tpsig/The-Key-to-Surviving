using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerIdentity))]
public class PlayerController : NetworkBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rb;
    private PlayerIdentity identity;
    private PlayerHealth health;

    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        identity = GetComponent<PlayerIdentity>();
        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!IsServer) return;

        input = GetInput();
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        rb.linearVelocity = input * speed;
    }

    Vector2 GetInput()
    {
        if (identity == null)
            return Vector2.zero;

        if (identity.role.Value == PlayerIdentity.PlayerRole.Player1)
        {
            return new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
        }

        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.LeftArrow)) x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) x = 1;
        if (Input.GetKey(KeyCode.UpArrow)) y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) y = -1;

        return new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Enemy"))
        {
            if (health != null)
            {
                health.TakeDamage(25);
            }
        }

        if (other.CompareTag("Key"))
        {
            GameManager.Instance.CollectKey();
        }
    }
}