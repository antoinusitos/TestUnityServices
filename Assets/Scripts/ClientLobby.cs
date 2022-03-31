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
        panelLobby = UILinker.instance.panelLobby;
        panelLobbyPlayer = panelLobby.transform.GetChild(0);
    }

    public void UpdatePlayerList(PlayerState[] playerStates)
    {
        for (int i = 0; i < playerStates.Length; i++)
        {
            if (playerStates[i].clientGUID == ClientPrefs.GetGuid())
            {
                //Show local player in the list
            }
            panelLobbyPlayer.GetChild(i).GetComponent<Text>().text = playerStates[i].playerName + "\t" + playerStates[i].kills + "\t" + playerStates[i].deaths;
        }
    }

    
}
