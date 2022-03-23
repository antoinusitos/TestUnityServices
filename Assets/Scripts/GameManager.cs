using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private string lobbyId;

    private RelayHostData hostData;
    private RelayJoinData joinData;

    private Lobby localLobby;

    public GameObject lookingText = null;
    public Text errorText = null;

    public GameObject serverText = null;
    public GameObject clientText = null;

    public Transform lobbiesPanel = null;
    public Button lobbyButton = null;

    async void Start()
    {
        // Initialize Unity Services
        await UnityServices.InitializeAsync();

        // Setup events listeners
        SetupEvents();

        // Unity Login
        await SignInAnonymouslyAsync();
    }

    public void StartClient()
    {
        FindMatch();
    }

    public void StarServer()
    {
        CreateMatch();
    }

    public void StarHost()
    {
        CreateMatch(true);
    }

    #region UnityLogin

    void SetupEvents()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            // Shows how to get a playerID
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

            // Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
        };

        AuthenticationService.Instance.SignInFailed += (err) =>
        {
            Debug.LogError(err);
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Player signed out.");
        };
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded !");

            lookingText.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #endregion

    #region Lobby

    private void HandleClientConnected(ulong clientId)
    {
        Debug.Log("Client Joined !");

        if(localLobby.Players.Count == localLobby.MaxPlayers)
        {
            Debug.Log("Lobby is full !");
        }
    }

    public async void FindMatch()
    {
        serverText.SetActive(false);
        clientText.SetActive(true);

        Debug.Log("Looking for a lobby...");
        errorText.text = "Looking for a lobby...";

        try
        {
            // Looking for a lobby

            // Add options to the matchmaking (mode, rank, etc.)
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions();

            QueryResponse query =  await Lobbies.Instance.QueryLobbiesAsync();
            errorText.text += "\n lobbies available :" + query.Results.Count;

            for(int i = 0; i < query.Results.Count; i++)
            {
                errorText.text += "\n " + query.Results[i].Id;
            }

            Lobby lobby = null;

            if (query.Results.Count > 0)
            {
                for (int i = 0; i < query.Results.Count; i++)
                {
                    Button button = Instantiate(lobbyButton, lobbiesPanel);
                    int index = i;
                    Lobby lobbyTemp = query.Results[index]; 
                    button.onClick.AddListener(async delegate
                    {
                        errorText.text += "\n joining by ID";

                        lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyTemp.Id);

                        Debug.Log("Joined lobby: " + lobby.Id);
                        Debug.Log("Lobby Players: " + lobby.Players.Count);

                        errorText.text += "\n Joined lobby";

                        // Retrieve the RELAY code previously set in the create match
                        string joinCode = lobby.Data["joinCode"].Value;

                        Debug.Log("Received code: " + joinCode);
                        errorText.text += "\n Joined lobby";

                        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

                        // Create Object
                        joinData = new RelayJoinData
                        {
                            Key = allocation.Key,
                            Port = (ushort)allocation.RelayServer.Port,
                            AllocationID = allocation.AllocationId,
                            AllocationIDBytes = allocation.AllocationIdBytes,
                            ConnectionData = allocation.ConnectionData,
                            HostConnectionData = allocation.HostConnectionData,
                            IPv4Address = allocation.RelayServer.IpV4
                        };

                        // Set Transports data
                        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                            joinData.IPv4Address,
                            joinData.Port,
                            joinData.AllocationIDBytes,
                            joinData.Key,
                            joinData.ConnectionData,
                            joinData.HostConnectionData);

                        // Finally start the client
                        NetworkManager.Singleton.StartClient();
                    });
                }

            }
            else
            {
                // Quick-join a random lobby
                errorText.text += "\n joining quick";
                lobby = await Lobbies.Instance.QuickJoinLobbyAsync(options);

                Debug.Log("Joined lobby: " + lobby.Id);
                Debug.Log("Lobby Players: " + lobby.Players.Count);

                errorText.text += "\n Joined lobby";

                // Retrieve the RELAY code previously set in the create match
                string joinCode = lobby.Data["joinCode"].Value;

                Debug.Log("Received code: " + joinCode);
                errorText.text += "\n Joined lobby";

                JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

                // Create Object
                joinData = new RelayJoinData
                {
                    Key = allocation.Key,
                    Port = (ushort)allocation.RelayServer.Port,
                    AllocationID = allocation.AllocationId,
                    AllocationIDBytes = allocation.AllocationIdBytes,
                    ConnectionData = allocation.ConnectionData,
                    HostConnectionData = allocation.HostConnectionData,
                    IPv4Address = allocation.RelayServer.IpV4
                };

                // Set Transports data
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinData.IPv4Address,
                    joinData.Port,
                    joinData.AllocationIDBytes,
                    joinData.Key,
                    joinData.ConnectionData,
                    joinData.HostConnectionData);

                // Finally start the client
                NetworkManager.Singleton.StartClient();
            }
        }
        catch (LobbyServiceException e)
        {
            // If we don't find any lobby, let's create a new one
            Debug.Log("Cannot find a lobby: " + e);
            errorText.text += "\n Cannot find a lobby: " + e;
            //CreateMatch();
        }
    }

    private async void CreateMatch(bool asHost = false)
    {
        serverText.SetActive(true);
        clientText.SetActive(false);

        Debug.Log("Creating a new lobby...");
        errorText.text = "Creating a new lobby...";

        // External Connections
        int maxConnections = 10;

        try
        {
            // Create RELAY Object
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxConnections);
            hostData = new RelayHostData
            {
                Key = allocation.Key,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                IPv4Address = allocation.RelayServer.IpV4
            };

            // Retrieve JoinCode, will let lobby players join this host via RELAY
            hostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            string lobbyName = "game_lobby";
            int maxPlayers = 10;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;

            // Put the JoinCode in the lobby data, visible by every member
            // Allow to share data between lobbu players (and external too)
            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "joinCode", new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: hostData.JoinCode)
                },
            };

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            
            lobbyId = lobby.Id;

            localLobby = lobby;

            Debug.Log("Created lobby: " + lobby.Id);
            errorText.text = "Created lobby: " + lobby.Id;

            // Heartbeat the lobby every 15 seconds to prevent Unity to shut it down.
            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            // Now that RELAY and LOBBY are set...

            // Set Transports data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                hostData.IPv4Address,
                hostData.Port,
                hostData.AllocationIDBytes,
                hostData.Key,
                hostData.ConnectionData);

            if(!asHost)
            {
                // Finally start server
                NetworkManager.Singleton.StartServer();
            }
            else
            {
                // Finally start host
                NetworkManager.Singleton.StartHost();
            }

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            errorText.text = e.ToString();
            throw;
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while(true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("lobby heartbit");
            yield return delay;
        }
    }

    private void OnDestroy()
    {
        // We need to delete the lobby when we are not using it
        Lobbies.Instance.DeleteLobbyAsync(lobbyId);
    }

    #endregion

    #region Relay

    // RelayHostData represents the necessary informations
    // for a Host to host a game on a Relay
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    // RelayHostData represents the necessary informations
    // for a Host to host a game on a Relay
    public struct RelayJoinData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }

    #endregion
}
