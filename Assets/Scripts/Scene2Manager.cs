using Unity.Netcode;
using UnityEngine;

public class Scene2Manager : NetworkBehaviour
{
    public float timer = 360;

    private void Start()
    {
        UILinker.instance.debugCanvas.SetActive(false);
        if(IsServer)
            ServerLobby.instance.scene2Manager = this;
    }

    private void Update()
    {
        if(IsServer)
        {
            timer -= Time.deltaTime;
        }
    }
}
