using Unity.Netcode;
using UnityEngine;

public class PlayerMenu : NetworkBehaviour
{
    public GameObject playerPrefab = null;

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

    public void SpawnPlayerForClients()
    {
        Debug.Log("Spawning");
        PlayerState[] playerStates = ServerLobby.instance.clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            GameObject go = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<NetworkObject>().ChangeOwnership(playerStates[i].clientID);
        }
    }
}
