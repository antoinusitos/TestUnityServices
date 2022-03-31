using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Transform cameraPivot = null;
    private Transform cameraTransform = null;
    private GameObject playerCanvas = null;

    private CharacterController characterController = null;

    private float speed = 5;
    private float rotationSpeed = 100;

    public PlayerUI playerUI = null;

    public Animator animator = null;

    public PlayerAnimationReplication animationReplication = null;

    private int leanSide = 0;

    private void Start()
    {
        if (!IsOwner)
            return;

        NetworkManager.Singleton.LocalClient.PlayerObject = GetComponent<NetworkObject>();

        FindObjectOfType<ObserverCamera>().gameObject.SetActive(false);
        cameraPivot = transform.GetChild(0);
        cameraPivot.gameObject.SetActive(true);
        cameraTransform = cameraPivot.GetChild(0);
        playerCanvas = transform.GetChild(1).gameObject;
        playerCanvas.SetActive(true);
        characterController = GetComponent<CharacterController>();
    }


    private void Update()
    {
        if (!IsOwner)
        {
            animator.SetFloat("Speed", animationReplication.currentSpeed.Value);
            animator.SetFloat("Direction", animationReplication.currentDirection.Value);

            if (animationReplication.currentLean.Value == -1)
            {
                animator.SetBool("LeanLeft", true);
                animator.SetBool("LeanRight", false);
            }
            else if (animationReplication.currentLean.Value == 1)
            {
                animator.SetBool("LeanRight", true);
                animator.SetBool("LeanLeft", false);
            }
            else
            {
                animator.SetBool("LeanLeft", false);
                animator.SetBool("LeanRight", false);
            }

            if(animationReplication.currentReload.Value)
            {
                animator.SetTrigger("Reload");
            }

            return;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            playerUI.ShowScore(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            playerUI.ShowScore(false);
        }

        if (Input.GetKey(KeyCode.A))
        {
            leanSide = -1;
            animator.SetBool("LeanLeft", true);
            animator.SetBool("LeanRight", false);
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * 25);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            leanSide = 1;
            animator.SetBool("LeanRight", true);
            animator.SetBool("LeanLeft", false);
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * -25);
        }
        else
        {
            leanSide = 0;
            animator.SetBool("LeanLeft", false);
            animator.SetBool("LeanRight", false);
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * 0);
        }

        animationReplication.UpdateLean(leanSide);

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

        animationReplication.UpdateSpeedAndDir(movement.z, movement.x);
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
