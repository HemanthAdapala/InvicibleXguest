using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    
    #region SINGLETON
    public static PlayerManager LocalInstance { get;private set; }
    
    #endregion
    
    [SerializeField] private Transform abilitiesHolderTransform;

    private int _playerCoins;
    public int PlayerCoins { get;private set; }
    
    private void Start()
    {
        if (!IsOwner) return;
        LocalInstance = this;
        Debug.Log("LocalInstance assigned to: " + gameObject.name);
        InitilizeCoinsData();
    }

    private void InitilizeCoinsData()
    {
        PlayerCoins = 300;
        Debug.Log("Player Coins Initialized:- " + PlayerCoins );
    }

    public void UpdateCoinsData(int ePrice)
    {
        PlayerCoins -= ePrice;
        Debug.Log("Player Coins Updated:- " + PlayerCoins );
    }
    
    // [Rpc(SendTo.Server)]
    // public void AddNetworkObjectToPlayerServerRpc(ItemType itemType,Vector3 position,Quaternion rotation,RpcParams rpcParams = default)
    // {
    //     if (abilityItemNetworkPrefabs.TryGetValue(itemType, out var abilityItemPrefab))
    //     {
    //         var networkObjectRef = Instantiate(abilityItemPrefab,position,rotation);
    //         var networkObject = networkObjectRef.GetComponent<NetworkObject>();
    //         networkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    //         var IAbilityNetworkHandler = networkObject.GetComponent<IAbilityNetworkHandler>();
    //         IAbilityNetworkHandler?.HideGameObjectFromAllClients();
    //         Debug.Log("NetworkObject Spawned:- " + networkObject.NetworkObjectId);
    //     }
    // }      


    public bool IsPlayerHasEnoughCoins(int itemPrice)
    {
        return PlayerCoins >= itemPrice;
    }
}
