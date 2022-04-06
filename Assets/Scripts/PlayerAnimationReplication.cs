using Unity.Netcode;

public class PlayerAnimationReplication : NetworkBehaviour
{
    public NetworkVariable<float> currentSpeed = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float speed = 0;

    public NetworkVariable<float> currentDirection = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float dir = 0;

    public NetworkVariable<float> currentbodyAngle = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float bodyAngle = 0;

    public NetworkVariable<float> currentLean = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float leanSide = 0;

    public NetworkVariable<bool> currentReload = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone, false);
    private int frameReload = 0;

    public NetworkVariable<bool> currentCrouch = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone, false);
    private bool crouch = false;

    public NetworkVariable<bool> currentDeath = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone, false);
    private bool death = false;

    private void Update()
    {
        if(IsServer)
        {
            if(currentReload.Value)
            {
                if(frameReload >= 1)
                {
                    currentReload.Value = false;
                    frameReload = 0;
                }
                frameReload++;
            }
        }
    }

    public void UpdateSpeedAndDir(float s, float d)
    {
        bool shouldUpdate = false;
        if (speed != s)
        {
            speed = s;
            shouldUpdate = true;
        }
        if (dir != d)
        {
            dir = d;
            shouldUpdate = true;
        }

        if(shouldUpdate)
        {
            UpdateSpeedAndDirServerRPC(speed, dir);
        }
    }

    [ServerRpc]
    private void UpdateSpeedAndDirServerRPC(float s, float d)
    {
        currentSpeed.Value = s;
        currentDirection.Value = d;
    }

    public void UpdateBodyAngle(float a)
    {
        bool shouldUpdate = false;
        if (bodyAngle != a)
        {
            bodyAngle = a;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            UpdateBodyAngleServerRPC(bodyAngle);
        }
    }

    [ServerRpc]
    private void UpdateBodyAngleServerRPC(float a)
    {
        currentbodyAngle.Value = a;
    }

    public void UpdateLean(float lean)
    {
        bool shouldUpdate = false;
        if (lean != leanSide)
        {
            leanSide = lean;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            UpdateLeanServerRPC(leanSide);
        }
    }

    [ServerRpc]
    private void UpdateLeanServerRPC(float lean)
    {
        currentLean.Value = lean;
    }

    public void UpdateCrouch(bool c)
    {
        bool shouldUpdate = false;
        if (c != crouch)
        {
            crouch = c;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            UpdateCrouchServerRPC(crouch);
        }
    }

    [ServerRpc]
    private void UpdateCrouchServerRPC(bool c)
    {
        currentCrouch.Value = c;
    }

    public void UpdateDeath(bool d)
    {
        bool shouldUpdate = false;
        if (d != death)
        {
            death = d;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            UpdateDeathServerRPC(death);
        }
    }

    [ServerRpc]
    private void UpdateDeathServerRPC(bool d)
    {
        currentDeath.Value = d;
    }

    public void UpdateReload()
    {
        UpdateReloadServerRPC();
    }

    [ServerRpc]
    private void UpdateReloadServerRPC()
    {
        currentReload.Value = true;
    }
}
