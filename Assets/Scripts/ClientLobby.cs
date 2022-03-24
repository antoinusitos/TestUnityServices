using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ClientLobby : NetworkBehaviour
{
    private Lobby localLobby = null;

    private NetworkClient localClient = null;

    private void Start()
    {
        localClient = NetworkManager.Singleton.LocalClient;
        RegisterToServer();
    }

    public void SetLobby(Lobby lobby)
    {
        localLobby = lobby;
    }

    private void RegisterToServer()
    {
        Debug.Log("RegisterToServer");
        SendClientToServer_ServerRpc(/*localClient, */localClient.ClientId);
    }


    [ServerRpc]
    private void SendClientToServer_ServerRpc(/*NetworkClient client, */ulong clientID)
    {
        //Debug.Log("Server receive : SendClientToServer");
        Debug.Log("clientID : " + clientID);
    }
}
