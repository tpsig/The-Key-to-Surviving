using UnityEngine;
using Unity.Netcode;

public class MenuManager : MonoBehaviour
{
    public GameManager gameManager;

    public void CreateParty()
    {
        gameManager.CreateParty();
    }

    public void JoinParty()
    {
        gameManager.JoinParty();
    }

    public void StartGame()
    {
        gameManager.StartGame();
    }
}