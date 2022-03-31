using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ServerLobby : NetworkBehaviour
{
    public static ServerLobby instance = null;

    public List<PlayerState> clients = new List<PlayerState>();

    private GameObject panelLobby = null;
    private Transform panelLobbyPlayer = null;

    public Transform playerLobbyTextPrefab = null;

    public GameObject playerPrefab = null;
    public GameObject observerPrefab = null;

    public Scene2Manager scene2Manager = null;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!IsServer)
            return;

        panelLobby = UILinker.instance.panelLobby;
        panelLobbyPlayer = panelLobby.transform.GetChild(0);
        panelLobbyPlayer.gameObject.SetActive(true);
        Travel();
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

        Transform instance = Instantiate(playerLobbyTextPrefab);
        instance.GetComponent<Text>().text = arrivingClient.playerName;
        instance.GetComponent<NetworkObject>().Spawn();
        instance.SetParent(panelLobbyPlayer);

        clients.Add(arrivingClient);

        SpawnObserverForClient(arrivingClient.clientID);
    }

    public void SpawnPlayerForClient(PlayerState playerState, int team)
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Player for " + playerState.clientID);

        PlayerState[] playerStates = ServerLobby.instance.clients.ToArray();
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

        SendClientTimeClientRpc(scene2Manager.timer, clientRpcParams);
    }

    public void SpawnObserverForClients()
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Observers");
        PlayerState[] playerStates = ServerLobby.instance.clients.ToArray();
        for (int i = 0; i < playerStates.Length; i++)
        {
            SpawnObserverForClient(playerStates[i].clientID);
        }
    }

    public void SpawnObserverForClient(ulong clientID)
    {
        if (!IsServer)
            return;

        Debug.Log("Spawning Observer");
        GameObject go = Instantiate(observerPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
        go.GetComponent<NetworkObject>().ChangeOwnership(clientID);
    }

    [ClientRpc]
    private void SendClientTimeClientRpc(float time, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        FindObjectOfType<PlayerUI>().time = time;
    }
}
