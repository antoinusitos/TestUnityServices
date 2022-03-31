using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    FFA,
    TDM,
    CTF
}

public class ServerLobby : NetworkBehaviour
{
    public static ServerLobby instance = null;

    public List<PlayerState> clients = new List<PlayerState>();

    public GameObject playerPrefab = null;
    public GameObject observerPrefab = null;

    public Scene2Manager scene2Manager = null;

    public GameMode gameMode = GameMode.FFA;

    public float timer = 360;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!IsServer)
            return;

        Travel();
    }

    private void Update()
    {
        if (IsServer)
        {
            timer -= Time.deltaTime;
        }
    }

    public void StartGame()
    {
        StartCoroutine("SendScores");
    }

    private void Travel()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.SceneManager.LoadScene("Demo", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void ReceiveClient(PlayerState arrivingClient)
    {
        if (!IsServer)
            return;

        clients.Add(arrivingClient);

        SpawnObserverForClient(arrivingClient);
    }

    public void ClientLeave(ulong clientID)
    {
        Debug.Log("Client " + clientID + " left the game");
        for(int i = 0; i < clients.Count; i++)
        {
            if(clients[i].clientID == clientID)
            {
                clients.RemoveAt(i);
                return;
            }
        }
    }

    public void SpawnPlayerForClient(PlayerState playerState, int team)
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Player for " + playerState.clientID);

        PlayerState[] playerStates = clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            if (playerState.clientGUID == playerStates[i].clientGUID)
            {
                GameObject go = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                go.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerStates[i].clientID);

                StartCoroutine(SendTime(playerStates[i].clientID));

                break;
            }
        }
    }

    private IEnumerator SendScores()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (true)
        {
            yield return waitForSeconds;
            SendClientTimeClientRpc(clients.ToArray());
        }
    }

    [ClientRpc]
    private void SendClientTimeClientRpc(PlayerState[] playerStates)
    {
        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
        if(playerUI != null)
            playerUI.UpdatePlayerList(playerStates);
    }

    private IEnumerator SendTime(ulong clientID)
    {
        yield return new WaitForSeconds(0.5f);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        SendClientTimeClientRpc(timer, gameMode, clientRpcParams);
    }

    public void SpawnObserverForClients()
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Observers");
        PlayerState[] playerStates = clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            SpawnObserverForClient(playerStates[i]);
        }
    }

    public void SpawnObserverForClient(PlayerState playerState)
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Observer");
        GameObject go = Instantiate(observerPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().ChangeOwnership(playerState.clientID);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerState.clientID }
            }
        };

        SendClientPlayerStateClientRpc(playerState, clientRpcParams);
    }

    [ClientRpc]
    private void SendClientTimeClientRpc(float time, GameMode gameMode, ClientRpcParams clientRpcParams = default)
    {
        PlayerUI playerUI = FindObjectOfType<PlayerUI>();
        playerUI.time = time;
        playerUI.SetGameMode(gameMode);
    }

    [ClientRpc]
    private void SendClientPlayerStateClientRpc(PlayerState playerState, ClientRpcParams clientRpcParams = default)
    {
        FindObjectOfType<ObserverCamera>().ReceivePlayerState(playerState);
    }

    public void UpdateKill(ulong clientID)
    {
        for(int i = 0; i < clients.Count; i++)
        {
            if(clients[i].clientID == clientID)
            {
                clients[i].kills++;
                return;
            }
        }
    }

    public void UpdateDeath(ulong clientID)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].clientID == clientID)
            {
                clients[i].deaths++;
                return;
            }
        }
    }
}
