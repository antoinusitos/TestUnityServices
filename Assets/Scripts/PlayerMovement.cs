using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    private Transform cameraPivot = null;
    private Transform cameraTransform = null;
    private GameObject playerCanvas = null;

    private CharacterController characterController = null;

    private float speed = 5;
    private float crouchSpeed = 2.5f;
    private float rotationSpeed = 150;

    private float cameraNormalHeight = 0.7f;
    private float cameraCrouchHeight = 0.25f;

    private float colliderNormalHeight = 1.7f;
    private float colliderCrouchHeight = 1.2f;

    private float colliderNormalY = 0;
    private float colliderCrouchY = -0.25f;

    public PlayerUI playerUI = null;

    public Animator animator = null;
    public Animator fpsAnimator = null;

    public PlayerAnimationReplication animationReplication = null;

    private int leanSide = 0;

    public GameObject playerModel = null;

    private float bodyAngle = 0;

    private bool crouch = false;
	
    private Player player = null;

    private float jumpForce = 5;

    private RespawnPoint[] respawnPoints = null;

    private bool canMoveCam = true;

    private void Start()
    {
        if (!IsOwner)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        player = GetComponent<Player>();
        NetworkManager.Singleton.LocalClient.PlayerObject = GetComponent<NetworkObject>();

        FindObjectOfType<ObserverCamera>().gameObject.SetActive(false);
        cameraPivot = transform.GetChild(0);
        cameraPivot.gameObject.SetActive(true);
        cameraTransform = cameraPivot.GetChild(0);
        playerCanvas = transform.GetChild(1).gameObject;
        playerCanvas.SetActive(true);
        characterController = GetComponent<CharacterController>();
        playerModel.SetActive(false);

        respawnPoints = FindObjectsOfType<RespawnPoint>();

        Respawn();
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

            animator.SetBool("Crouch", animationReplication.currentCrouch.Value);

            return;
        }

        if (player.GetIsInPause())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
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

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = !crouch;
            if (crouch)
            {
                cameraTransform.localPosition = Vector3.up * cameraCrouchHeight;
                characterController.height = colliderCrouchHeight;
                characterController.center = Vector3.up * colliderCrouchY;
            }
            else
            {
                cameraTransform.localPosition = Vector3.up * cameraNormalHeight;
                characterController.height = colliderNormalHeight;
                characterController.center = Vector3.up * colliderNormalY;
            }
        }
        animationReplication.UpdateCrouch(crouch);

        fpsAnimator.SetFloat("LeanSide", leanSide);
        animationReplication.UpdateLean(leanSide);

        if(characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                characterController.attachedRigidbody.AddForce(Vector3.up * jumpForce);
            }
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

        animationReplication.UpdateSpeedAndDir(movement.z, movement.x);
        fpsAnimator.SetFloat("Speed", movement.z);
        fpsAnimator.SetFloat("Direction", movement.x);

        float currentSpeed = crouch ? crouchSpeed : speed;
        characterController.SimpleMove(movement * currentSpeed);

        if (canMoveCam)
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            bodyAngle += -deltaY * Time.deltaTime * rotationSpeed;
            if (bodyAngle > 89)
                bodyAngle = 89;
            else if (bodyAngle < -89)
                bodyAngle = -89;
            animationReplication.UpdateBodyAngle(-bodyAngle / 90.0f);

            transform.Rotate(Vector3.up * deltaX * Time.deltaTime * rotationSpeed);
            cameraTransform.localRotation = Quaternion.Euler(Vector3.right * bodyAngle);
        }
    }

    public void SetCanMoveCam(bool newState)
    {
        canMoveCam = newState;
    }

    //On Client
    public void Respawn()
    {
        characterController.enabled = false;

        if (PlayerInfos.instance.currentPlayerState.team != 0)
        {
            for (int i = 0; i < respawnPoints.Length; i++)
            {
                if (respawnPoints[i].team == PlayerInfos.instance.currentPlayerState.team)
                {
                    transform.SetPositionAndRotation(respawnPoints[i].transform.position, respawnPoints[i].transform.rotation);
                    characterController.enabled = true;
                    return;
                }
            }
        }

        //if we didn't find any spawn point
        int rand = Random.Range(0, respawnPoints.Length);
        transform.SetPositionAndRotation(respawnPoints[rand].transform.position, respawnPoints[rand].transform.rotation);
        characterController.enabled = true;
    }
}
