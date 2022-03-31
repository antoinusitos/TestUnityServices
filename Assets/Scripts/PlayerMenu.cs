using Unity.Netcode;

public class PlayerMenu : NetworkBehaviour
{
    private void Start()
    {
        if(IsLocalPlayer)
        {
            UILinker.instance.panelLobby.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }
    }

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
}
