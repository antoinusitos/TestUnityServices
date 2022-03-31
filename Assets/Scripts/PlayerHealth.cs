using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public float maxLife = 100;
    public float life = 100;

    public PlayerUI playerUI = null;

    //On Server
    public void TakeDamage(float damage, ulong clientID, ulong senderID)
    {
        life -= damage;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        if (life <= 0)
        {
            ServerLobby.instance.UpdateKill(senderID);
            ServerLobby.instance.UpdateDeath(clientID);

            life = maxLife;
            RespawnClientRPC(clientRpcParams);
        }
        else
        {
            SendClientLifeClientRpc(life, clientRpcParams);
        }
    }

    [ClientRpc]
    private void SendClientLifeClientRpc(float life_In, ClientRpcParams clientRpcParams = default)
    {
        life = life_In;
        playerUI.UpdateLife(life, maxLife);
    }

    [ClientRpc]
    private void RespawnClientRPC(ClientRpcParams clientRpcParams = default)
    {
        life = maxLife;
        playerUI.UpdateLife(life, maxLife);

        GetComponent<PlayerMovement>().Respawn();
    }
}
