using Unity.Netcode;
using UnityEngine;

public class PlayerMenu : NetworkBehaviour
{
    public GameObject playerPrefab = null;
    public GameObject observerPrefab = null;

    [ServerRpc]
    public void RequestPlayerListServerRpc()
    {
        UpdateClientsPlayerList();
    }
  

    private void UpdateClientsPlayerList()
    {
        UpdateClientListClientRpc(ServerLobby.instance.clients.ToArray());
    }

    [ClientRpc]
    private void UpdateClientListClientRpc(PlayerState[] playerStates)
    {
        ClientLobby.instance.UpdatePlayerList(playerStates);
    }

    public void SpawnPlayerForClient(PlayerState playerState)
    {
        Debug.Log("Spawning Player for " + playerState.clientID);

        PlayerState[] playerStates = ServerLobby.instance.clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            if(playerState.clientGUID == playerStates[i].clientGUID)
            {
                GameObject go = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                go.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerStates[i].clientID);
                break;
            }
        }
    }

    public void SpawnObserverForClients()
    {
        Debug.Log("Spawning Observer");
        PlayerState[] playerStates = ServerLobby.instance.clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            GameObject go = Instantiate(observerPrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<NetworkObject>().ChangeOwnership(playerStates[i].clientID);
        }
    }
}
