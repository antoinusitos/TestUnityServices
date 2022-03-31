using Unity.Netcode;

public class PlayerMenu : NetworkBehaviour
{
    public PlayerState localPlayerState = null;

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
}
