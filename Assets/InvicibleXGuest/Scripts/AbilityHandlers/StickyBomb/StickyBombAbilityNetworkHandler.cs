using Unity.Netcode;
using UnityEngine;

public class StickyBombAbilityNetworkHandler : NetworkBehaviour, IAbilityNetworkHandler
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("Spawning the network object");
        }
    }

    public void HideGameObjectFromAllClients()
    {
        Debug.Log("Hiding the game object");
        HideGameObjectFromAllClientsRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void HideGameObjectFromAllClientsRpc()
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out var networkObject))
        {
            bool isOwner = NetworkManager.Singleton.LocalClientId == NetworkObject.OwnerClientId;
    
            foreach (var renderer in networkObject.GetComponents<MeshRenderer>())
            {
                renderer.enabled = isOwner;
            }
        }
    }
    
    // [Rpc(SendTo.SpecifiedInParams)]
    // private void DestroyTheVisualObjectRpc(RpcParams rpcParams = default)
    // {
    //     Debug.Log("Destroying the game object");
    //     if (_attachedPowerUpObjectVisual != null)
    //     {
    //         Destroy(_attachedPowerUpObjectVisual);
    //         _attachedPowerUpObjectVisual = null;
    //     }
    // }
    
    // protected override void OnPowerUpObjectPlaced(Vector3 position, Quaternion rotation)
    // {
    //     // Override the MonoBehaviour logic to call the server-side placement RPC
    //     SpawnPowerUpObjectRpc(position, rotation);
    // }

}
