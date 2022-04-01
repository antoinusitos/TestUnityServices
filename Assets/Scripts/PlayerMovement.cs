using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Transform cameraPivot = null;
    private Transform cameraTransform = null;
    private GameObject playerCanvas = null;

    private CharacterController characterController = null;

    private float speed = 5;
    private float rotationSpeed = 150;

    public PlayerUI playerUI = null;

    public Animator animator = null;

    public PlayerAnimationReplication animationReplication = null;

    private int leanSide = 0;

    public GameObject playerModel = null;

    private float bodyAngle = 0;

    private void Start()
    {
        if (!IsOwner)
            return;

        Cursor.lockState = CursorLockMode.Locked;

        NetworkManager.Singleton.LocalClient.PlayerObject = GetComponent<NetworkObject>();

        FindObjectOfType<ObserverCamera>().gameObject.SetActive(false);
        cameraPivot = transform.GetChild(0);
        cameraPivot.gameObject.SetActive(true);
        cameraTransform = cameraPivot.GetChild(0);
        playerCanvas = transform.GetChild(1).gameObject;
        playerCanvas.SetActive(true);
        characterController = GetComponent<CharacterController>();
        playerModel.SetActive(false);
    }


    private void Update()
    {
        if (!IsOwner)
        {
            animator.SetFloat("Speed", animationReplication.currentSpeed.Value);
            animator.SetFloat("Direction", animationReplication.currentDirection.Value);

            animator.SetFloat("LeanSide", animationReplication.currentLean.Value);
            animator.SetFloat("LookUpAngle", animationReplication.currentbodyAngle.Value);

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
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * 25);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            leanSide = 1;
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * -25);
        }
        else
        {
            leanSide = 0;
            cameraPivot.localRotation = Quaternion.Euler(Vector3.forward * 0);
        }

        animator.SetFloat("LeanSide", leanSide);
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

        bodyAngle += -deltaY * Time.deltaTime* rotationSpeed;
        if (bodyAngle > 89)
            bodyAngle = 89;
        else if (bodyAngle < -89)
            bodyAngle = -89;
        animationReplication.UpdateBodyAngle(-bodyAngle / 90.0f);

        transform.Rotate(Vector3.up * deltaX * Time.deltaTime * rotationSpeed);
        //cameraTransform.Rotate(Vector3.right * -deltaY * Time.deltaTime * rotationSpeed);
        cameraTransform.localRotation = Quaternion.Euler(Vector3.right * bodyAngle);
    }

    //On Client
    public void Respawn()
    {
        transform.position = Vector3.up;
    }
}
