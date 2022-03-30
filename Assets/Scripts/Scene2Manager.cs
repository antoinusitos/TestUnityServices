using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Scene2Manager : NetworkBehaviour
{
    private void Start()
    {
        GameObject.Find("LobbyCanvas").SetActive(false);
        GameObject.Find("DebugCanvas").SetActive(false);
        StartCoroutine("StartingGame");
    }

    private IEnumerator StartingGame()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(2);
            FindObjectOfType<PlayerMenu>().SpawnObserverForClients();
        }
    }
}
