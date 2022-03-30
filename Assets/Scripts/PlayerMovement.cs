using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Transform cameraTransform = null;

    private NetworkTransform networkTransform = null;

    private CharacterController characterController = null;

    private float speed = 10;
    private float rotationSpeed = 50;

    private void Start()
    {
        if (!IsOwner)
            return;

        NetworkManager.Singleton.LocalClient.PlayerObject = GetComponent<NetworkObject>();

        FindObjectOfType<ObserverCamera>().gameObject.SetActive(false);
        cameraTransform = transform.GetChild(0);
        cameraTransform.gameObject.SetActive(true);
        networkTransform = GetComponent<NetworkTransform>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
        {
            movement += transform.forward;
            //transform.position += cameraTransform.forward * Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
            //transform.position -= cameraTransform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            movement -= transform.right;
            //transform.position -= cameraTransform.right * Time.deltaTime * speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
            //transform.position += cameraTransform.right * Time.deltaTime * speed;
        }

        characterController.SimpleMove(movement * 5.0f);

        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * deltaX * Time.deltaTime * rotationSpeed);
        cameraTransform.Rotate(Vector3.right * -deltaY * Time.deltaTime * rotationSpeed);
    }
}
