using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Transform cameraTransform = null;
    private GameObject playerCanvas = null;

    private CharacterController characterController = null;

    private float speed = 5;
    private float rotationSpeed = 100;

    public PlayerUI playerUI = null;

    public Animator animator = null;

    private void Start()
    {
        if (!IsOwner)
            return;

        NetworkManager.Singleton.LocalClient.PlayerObject = GetComponent<NetworkObject>();

        FindObjectOfType<ObserverCamera>().gameObject.SetActive(false);
        cameraTransform = transform.GetChild(0);
        cameraTransform.gameObject.SetActive(true);
        playerCanvas = transform.GetChild(1).gameObject;
        playerCanvas.SetActive(true);
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            playerUI.ShowScore(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            playerUI.ShowScore(false);
        }

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
        {
            movement += transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            movement -= transform.right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }

        animator.SetFloat("Speed", movement.z);
        animator.SetFloat("Direction", movement.x);

        characterController.SimpleMove(movement * speed);

        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * deltaX * Time.deltaTime * rotationSpeed);
        cameraTransform.Rotate(Vector3.right * -deltaY * Time.deltaTime * rotationSpeed);
    }

    //On Client
    public void Respawn()
    {
        transform.position = Vector3.up;
    }
}
