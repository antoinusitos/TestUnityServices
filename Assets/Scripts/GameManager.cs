using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject lookingText = null;
    public Text errorText = null;

    public GameObject serverText = null;
    public GameObject clientText = null;

    public Button lobbyButton = null;

    public GameObject roleSelectionObject = null;

    public ServerLobby serverLobbyPrefab = null;

    public InputField clientAddressField = null;
    public InputField clientPortField = null;

    public InputField serverAddressField = null;
    public InputField serverPortField = null;

    private PlayerState sentPlayerState = null;

    private ServerLobby serverLobby = null;

    public MainMenu mainMenuObject = null;

    private bool canCreateLobby = false;
    public Text createLobbyErrorText = null;
    public Text joinLobbyErrorText = null;

    async void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += ServerStartedHandler;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

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
        CreateMatch();
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

            roleSelectionObject.SetActive(true);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    #endregion

    #region Lobby

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate connectionApprovedCallback)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("Server Approval");
            return;
        }

        string payload = System.Text.Encoding.UTF8.GetString(connectionData);
        PlayerState connectionPayload = JsonUtility.FromJson<PlayerState>(payload);

        connectionApprovedCallback(true, null, true, Vector3.zero, Quaternion.identity);

        if(serverLobby != null)
        {
            Debug.Log("Receive client :" + clientId);
            connectionPayload.clientID = clientId;
            serverLobby.ReceiveClient(connectionPayload);
        }
    }

    private void ServerStartedHandler()
    {
        StartCoroutine("HandleServerStarted");
    }

    private IEnumerator HandleServerStarted()
    {
        yield return new WaitForSeconds(1);

        if (canCreateLobby)
        {
            Debug.Log("Server Started");
            errorText.text = "Server Started";

            serverLobby = Instantiate(serverLobbyPrefab);
            serverLobby.GetComponent<NetworkObject>().Spawn();
            gameObject.SetActive(false);
        }
    }

    public void FindMatch()
    {
        serverText.SetActive(false);
        clientText.SetActive(true);
        roleSelectionObject.SetActive(false);

        Debug.Log("Looking for a lobby...");
        errorText.text = "Looking for a lobby...";

        joinLobbyErrorText.text = "";

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = clientAddressField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(clientPortField.text);

        ConnectClient();
    }

    private void OnClientConnected(ulong clientID)
    {
        Debug.Log("OnClientConnected");
        StartCoroutine("HandleClientConnected");
    }

    private IEnumerator HandleClientConnected()
    {
        yield return new WaitForSeconds(1);
        if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Local Client detected");
            gameObject.SetActive(false);
        }
    }

    private void OnClientDisconnected(ulong clientID)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            ServerLobby.instance.ClientLeave(clientID);
        }
    }

    private void ConnectClient()
    {
        Debug.Log("Connecting Client");

        var clientGuid = ClientPrefs.GetGuid();
        sentPlayerState = new PlayerState(0, clientGuid, PlayerPrefs.GetString("PlayerName"));
        
        string payload = JsonUtility.ToJson(sentPlayerState);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // Finally start the client
        canCreateLobby = NetworkManager.Singleton.StartClient();

        if(canCreateLobby)
        {
            Debug.Log("joining");
            mainMenuObject.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot create Lobby");
            errorText.text = "Cannot create Lobby";
            joinLobbyErrorText.text = "Cannot Create lobby, check your Server Address or Port";
        }
    }

    private void CreateMatch()
    {
        Debug.Log("Creating a new lobby...");
        errorText.text = "Creating a new lobby...";

        createLobbyErrorText.text = "";

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = serverAddressField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(serverPortField.text);

        canCreateLobby = NetworkManager.Singleton.StartServer();

        if (canCreateLobby)
        {
            Debug.Log("Lobby created");
            mainMenuObject.gameObject.SetActive(false);
            serverText.SetActive(true);
            clientText.SetActive(false);
            roleSelectionObject.SetActive(false);
        }
        else
        {
            Debug.Log("Cannot create Lobby");
            errorText.text = "Cannot create Lobby";
            createLobbyErrorText.text = "Cannot Create lobby, check your Server Address or Port";
        }
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnServerStarted -= ServerStartedHandler;
        }
    }

    #endregion
}
