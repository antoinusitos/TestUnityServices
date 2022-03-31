using UnityEngine;

public class UILinker : MonoBehaviour
{
    public static UILinker instance = null;

    public GameObject panelLobby = null;
    public GameObject debugCanvas = null;

    private void Awake()
    {
        instance = this;
    }
}
