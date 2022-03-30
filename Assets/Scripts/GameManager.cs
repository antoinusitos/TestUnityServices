using System;
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

    public Transform lobbiesPanel = null;
    public Button lobbyButton = null;

    public GameObject roleSelectionObject = null;
    public GameObject serverSelectionObject = null;

    public Text serverNumbers = null;

    public ServerLobby serverLobbyPrefab = null;

    public ClientLobby clientLobbyPrefab = null;

    public InputField playerNameField = null;

    public InputField clientAddressField = null;
    public InputField clientPortField = null;

    public InputField serverAddressField = null;
    public InputField serverPortField = null;

    private PlayerState sentPlayerState = null;

    private ServerLobby serverLobby = null;

    async void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += ServerStartedHandler;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

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
            return;
        }

        string payload = System.Text.Encoding.UTF8.GetString(connectionData);
        PlayerState connectionPayload = JsonUtility.FromJson<PlayerState>(payload);

        connectionApprovedCallback(true, null, true, Vector3.zero, Quaternion.identity);

        if(serverLobby != null)
        {
            connectionPayload.clientID = clientId;
            serverLobby.ReceiveClient(connectionPayload);
        }
    }

    private void ServerStartedHandler()
    {
        Debug.Log("Server Started");
        errorText.text = "Server Started";

        serverLobby = Instantiate(serverLobbyPrefab);
        gameObject.SetActive(false);
    }

    public void FindMatch()
    {
        serverText.SetActive(false);
        clientText.SetActive(true);
        roleSelectionObject.SetActive(false);

        Debug.Log("Looking for a lobby...");
        errorText.text = "Looking for a lobby...";

        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = clientAddressField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(clientPortField.text);

        ConnectClient();
    }

    private void OnClientConnected(ulong clientID)
    {
        if(NetworkManager.Singleton.IsClient && NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log("Local Client detected");
            Instantiate(clientLobbyPrefab, GameObject.Find("PlayerMenu(Clone)").transform);
            gameObject.SetActive(false);
        }
    }

    private void ConnectClient()
    {
        Debug.Log("Connecting Client");

        var clientGuid = ClientPrefs.GetGuid();
        sentPlayerState = new PlayerState(0, clientGuid, playerNameField.text);
        
        string payload = JsonUtility.ToJson(sentPlayerState);
        byte[] payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        // Finally start the client
        NetworkManager.Singleton.StartClient();
    }

    private void CreateMatch()
    {
        serverText.SetActive(true);
        clientText.SetActive(false);
        roleSelectionObject.SetActive(false);

        Debug.Log("Creating a new lobby...");
        errorText.text = "Creating a new lobby...";


        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = serverAddressField.text;
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(serverPortField.text);

        NetworkManager.Singleton.StartServer();

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
