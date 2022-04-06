using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public float maxLife = 100;
    public float life = 100;

    public PlayerUI playerUI = null;

    public Transform cameraPivot = null;

    private Vector3 baseMeshPos = Vector3.zero;

    public Animator animator = null;
    public GameObject deathCam = null;
    public Camera fpsCam = null;
    public AudioListener fpsAudioListener = null;

    private PlayerMovement playerMovement = null;

    public PlayerAnimationReplication animationReplication = null;

    private void Start()
    {
        baseMeshPos = animator.transform.localPosition;
        playerMovement = GetComponent<PlayerMovement>();
    }

    //On Server
    public void TakeDamage(float damage, ulong clientID, ulong senderID)
    {
        life -= damage;

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        if (life <= 0)
        {
            ServerLobby.instance.UpdateKill(senderID);
            ServerLobby.instance.UpdateDeath(clientID);

            StartCoroutine(RespawnTimer(clientRpcParams));
        }

        SendClientLifeClientRpc(life, clientRpcParams);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && IsOwner)
        {
            KillSelfServerRPC(OwnerClientId);
        }
    }

    [ServerRpc]
    private void KillSelfServerRPC(ulong clientID)
    {
        TakeDamage(100, clientID, clientID);
    }

    [ClientRpc]
    private void SendClientLifeClientRpc(float life_In, ClientRpcParams clientRpcParams = default)
    {
        life = life_In;
        playerUI.UpdateLife(life, maxLife);

        if(life <= 0)
        {
            animationReplication.UpdateDeath(true);
            playerMovement.SetCanMoveCam(false);
            fpsCam.enabled = false;
            fpsAudioListener.enabled = false;
            animator.applyRootMotion = true;
            animator.SetBool("Death", true);
            deathCam.SetActive(true);
        }
    }

    private IEnumerator RespawnTimer(ClientRpcParams clientRpcParams)
    {
        yield return new WaitForSeconds(5);
        life = maxLife;
        RespawnClientRPC(clientRpcParams);
    }

    [ClientRpc]
    private void RespawnClientRPC(ClientRpcParams clientRpcParams = default)
    {
        animator.transform.localPosition = baseMeshPos;
        animator.transform.localRotation = Quaternion.identity;

        life = maxLife;
        playerUI.UpdateLife(life, maxLife);

        animationReplication.UpdateDeath(false);
        playerMovement.SetCanMoveCam(true);
        fpsCam.enabled = true;
        fpsAudioListener.enabled = true;
        animator.applyRootMotion = false;
        animator.SetBool("Death", false);
        deathCam.SetActive(false);


        GetComponent<PlayerMovement>().Respawn();
    }
}
