using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ServerLobby : NetworkBehaviour
{
    public List<ulong> clients = new List<ulong>();

    public Lobby currentLobby = null;

    public void ReceiveClient(ulong clientId)
    {
        Debug.Log("ReceiveClient:" + clientId);

        clients.Add(clientId);

        if (currentLobby.Players.Count == currentLobby.MaxPlayers)
        {
            Debug.Log("Lobby is full !");
        }
    }
}
