/*using Unity.Netcode;
using UnityEngine;

public class PlayerAssigner : NetworkBehaviour
{
    public PlayerIdentity player1;
    public PlayerIdentity player2;

    private bool assigned = false;

    public override void OnNetworkSpawn()
    {
        TryAssign();
    }

    private void Update()
    {
        if (IsServer && !assigned)
        {
            TryAssign();
        }
    }

    private void TryAssign()
    {
        Debug.Log($"P1 Spawned: {player1.IsSpawned}");
        Debug.Log($"P2 Spawned: {player2.IsSpawned}");
        if (player1 == null || player2 == null) return;

        // IMPORTANT: only assign when spawned
        if (!player1.IsSpawned || !player2.IsSpawned) return;

        player1.playerId.Value = 1;
        player2.playerId.Value = 2;

        assigned = true;
    }
}
*/