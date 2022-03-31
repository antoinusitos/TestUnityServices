using UnityEngine;

public class PlayerInfos : MonoBehaviour
{
    public static PlayerInfos instance = null;

    public PlayerState currentPlayerState = null;

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerState(PlayerState playerState)
    {
        currentPlayerState = playerState;
    }
}
