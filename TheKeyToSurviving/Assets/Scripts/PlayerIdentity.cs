using Unity.Netcode;

public class PlayerIdentity : NetworkBehaviour
{
    public enum PlayerRole
    {
        Player1,
        Player2
    }

    public NetworkVariable<PlayerRole> role = new NetworkVariable<PlayerRole>();
}