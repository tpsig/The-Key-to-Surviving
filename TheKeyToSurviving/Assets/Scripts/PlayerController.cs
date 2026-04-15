using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerIdentity))]
public class PlayerController : NetworkBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rb;
    private PlayerIdentity identity;

    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        identity = GetComponent<PlayerIdentity>();
    }

    void Update()
    {
        if (!IsOwner) return;

        input = GetInput();
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

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

    // ======================================================
    // ENEMY + KEY UNIFIED DETECTION (CLIENT SIDE TRIGGER)
    // ======================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        // -------------------------
        // KEY INTERACTION
        // -------------------------
        Key key = other.GetComponent<Key>();
        if (key != null)
        {
            InteractServerRpc(key.NetworkObject);
            return;
        }

        // -------------------------
        // ENEMY INTERACTION
        // -------------------------
        if (other.CompareTag("Enemy"))
        {
            EnemyHitServerRpc();
        }
    }

    // ======================================================
    // SERVER HANDLES EVERYTHING (AUTHORITATIVE)
    // ======================================================

    [ServerRpc]
    private void InteractServerRpc(NetworkObjectReference keyRef)
    {
        if (!keyRef.TryGet(out NetworkObject netObj))
            return;

        Key key = netObj.GetComponent<Key>();
        if (key == null)
            return;

        GameManager.Instance.CollectKey();
        netObj.Despawn(true);
    }

    [ServerRpc]
    private void EnemyHitServerRpc()
    {
        PlayerHealth health = GetComponent<PlayerHealth>();

        if (health == null)
        {
            Debug.LogError("Missing PlayerHealth");
            return;
        }

        Debug.Log($"[SERVER] Damaging Player Owner={OwnerClientId}");

        health.TakeDamage(25);

        // simple knockback (safe version)
        Vector2 knockDir = Vector2.zero;

        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy != null)
        {
            knockDir = (transform.position - enemy.transform.position).normalized;
        }

        Knockback(knockDir, 5f);
    }

    // ======================================================
    // MOVEMENT SUPPORT (UNCHANGED)
    // ======================================================

    public void Knockback(Vector2 direction, float force)
    {
        if (!IsServer) return;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        GameObject spawn = (OwnerClientId == 0)
            ? GameObject.FindWithTag("HostSpawn")
            : GameObject.FindWithTag("ClientSpawn");

        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
}