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

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        panelLobby = GameObject.Find("LobbyCanvas");
        panelLobbyPlayer = panelLobby.transform.GetChild(0);
        panelLobbyPlayer.gameObject.SetActive(true);
    }

    public void ReceiveClient(PlayerState arrivingClient)
    {
        Transform instance = Instantiate(playerLobbyTextPrefab);
        instance.GetComponent<Text>().text = arrivingClient.playerName;
        instance.GetComponent<NetworkObject>().Spawn();
        instance.SetParent(panelLobbyPlayer);

        clients.Add(arrivingClient);

        StartCoroutine("Travel");
    }

    private IEnumerator Travel()
    {
        yield return new WaitForSeconds(2);
        NetworkManager.Singleton.SceneManager.LoadScene("Scene2", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
