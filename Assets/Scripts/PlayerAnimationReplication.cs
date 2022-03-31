using Unity.Netcode;

public class PlayerAnimationReplication : NetworkBehaviour
{
    public NetworkVariable<float> currentSpeed = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float speed = 0;

    public NetworkVariable<float> currentDirection = new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 0);
    private float dir = 0;

    public NetworkVariable<int> currentLean = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 0);
    private int leanSide = 0;

    public NetworkVariable<bool> currentReload = new NetworkVariable<bool>(NetworkVariableReadPermission.Everyone, false);
    private int frameReload = 0;

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

    public void UpdateLean(int lean)
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
    private void UpdateLeanServerRPC(int lean)
    {
        currentLean.Value = lean;
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
