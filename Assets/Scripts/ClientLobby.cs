using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ClientLobby : NetworkBehaviour
{
    public static ClientLobby instance = null;

    public PlayerState localPlayerState = null;

    private GameObject panelLobby = null;
    private Transform panelLobbyPlayer = null;

    public Text playerLobbyTextPrefab = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        panelLobby = GameObject.Find("LobbyCanvas");
        panelLobbyPlayer = panelLobby.transform.GetChild(0);
        panelLobbyPlayer.gameObject.SetActive(true);

        StartCoroutine("RequestPlayerList");
    }

    private IEnumerator RequestPlayerList()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<PlayerMenu>().RequestPlayerListServerRpc();
    }

    public void UpdatePlayerList(PlayerState[] playerStates)
    {
        for (int i = 0; i < playerStates.Length; i++)
        {
            panelLobbyPlayer.GetChild(i).GetComponent<Text>().text = playerStates[i].playerName;
        }
    }

    
}
