using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public override void OnGainedOwnership()
    {
        base.OnGainedOwnership();
        Debug.Log("OnGainedOwnership");
        Camera.main.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("lool");
        }
    }
}
