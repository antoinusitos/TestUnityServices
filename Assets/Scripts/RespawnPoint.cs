using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public int team = 0;

    private void Awake()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
