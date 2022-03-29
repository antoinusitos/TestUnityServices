using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Scene2Manager : NetworkBehaviour
{
    private void Start()
    {
        StartCoroutine("StartingGame");
    }

    private IEnumerator StartingGame()
    {
        if (IsServer)
        {
            yield return new WaitForSeconds(2);
            Debug.Log("Spawning players");
            FindObjectOfType<PlayerMenu>().SpawnPlayerForClients();
        }
    }
}
