using Unity.Netcode;
using UnityEngine;

namespace InvicibleXGuest.Scripts
{
    public class GameObjectVisibilityHandler : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                // Only let the owner see this object
                //NetworkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
            }
            else
            {
                // Remove from non-owner clients
                //NetworkObject.NetworkHide();
            }
        }
    }
}