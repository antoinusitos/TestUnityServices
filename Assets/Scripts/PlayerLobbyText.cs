using Unity.Netcode;

public class PlayerLobbyText : NetworkBehaviour
{
    /// <summary>
    /// Gets called when the parent NetworkObject of this NetworkBehaviour's NetworkObject has changed
    /// </summary>
    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject) 
    {
        if(parentNetworkObject != null && transform != null)
        {
            transform.SetParent(parentNetworkObject.transform);
        }
    }
}
