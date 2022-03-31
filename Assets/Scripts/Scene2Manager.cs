using Unity.Netcode;

public class Scene2Manager : NetworkBehaviour
{
    private void Start()
    {
        UILinker.instance.debugCanvas.SetActive(false);

        if(IsServer)
        {
            ServerLobby.instance.StartGame();
        }
    }
}