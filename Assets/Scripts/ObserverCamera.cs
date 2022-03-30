using Unity.Netcode;
using UnityEngine;

public class ObserverCamera : NetworkBehaviour
{
    private float speed = 10;
    private float rotationSpeed = 50;
    private Transform cameraTransform = null;

    public GameObject charSelectionCanvas = null;

    public override void OnGainedOwnership()
    {
        base.OnGainedOwnership();

        if (IsOwner)
        {
            Debug.Log("OnGainedOwnership");
            Camera.main.gameObject.SetActive(false);
            cameraTransform = transform.GetChild(0);
            cameraTransform.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!charSelectionCanvas.activeSelf)
        {

            if (Input.GetKey(KeyCode.Z))
            {
                transform.position += cameraTransform.forward * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.position -= cameraTransform.forward * Time.deltaTime * speed;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                transform.position -= cameraTransform.right * Time.deltaTime * speed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += cameraTransform.right * Time.deltaTime * speed;
            }

            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            transform.Rotate(Vector3.up * deltaX * Time.deltaTime * rotationSpeed);
            cameraTransform.Rotate(Vector3.right * -deltaY * Time.deltaTime * rotationSpeed);

            if (Input.GetMouseButtonDown(0))
            {
                charSelectionCanvas.SetActive(true);
            }
        }
    }

    public void ChoseTeam(int team)
    {
        Debug.Log("chose team:" + team);
        ChoseTeamServerRPC(team, FindObjectOfType<ClientLobby>().localPlayerState);
        FindObjectOfType<PlayerMenu>().gameObject.SetActive(false);
    }

    [ServerRpc]
    public void ChoseTeamServerRPC(int team, PlayerState playerState)
    {
        FindObjectOfType<PlayerMenu>().SpawnPlayerForClient(playerState);
    }
}