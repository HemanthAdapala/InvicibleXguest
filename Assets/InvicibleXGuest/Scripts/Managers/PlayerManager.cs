using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using InvicibleXGuest.Scripts.Interfaces;
using InvicibleXGuest.Scripts.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : NetworkBehaviour
{
    
    #region SINGLETON
    public static PlayerManager LocalInstance { get;private set; }
    
    #endregion
    
    [SerializeField] private Transform abilitiesHolderTransform;

    private int _playerCoins;
    public int PlayerCoins { get;private set; }
    
    [SerializeField] public SerializedDictionary<AbilityItemType,GameObject> abilityItemNetworkPrefabs = new();

    //Serialized Dictionary to store the player's ability items
    private SerializedDictionary<AbilityItemType, AbilityItemData> _playerAbilityItemData;
    
    //Dictionary to store the player's abilities
    private Dictionary<Type, IAbilityHandler> _abilities = new Dictionary<Type, IAbilityHandler>();

    public event Action<Type> OnAbilityAdded;
    public event Action<Type> OnAbilityRemoved;
    
    private void Start()
    {
        if (!IsOwner) return;
        LocalInstance = this;
        Debug.Log("LocalInstance assigned to: " + gameObject.name);
        InitilizeCoinsData();
        _playerAbilityItemData = new SerializedDictionary<AbilityItemType, AbilityItemData>();
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

    public void AddToEquipItems(AbilityItemType itemType, AbilityItemData abilityItemData)
    {
        _playerAbilityItemData.Add(itemType,abilityItemData);
        SetPlayerAbilityItemPurchasedStatus(itemType, true);
        Debug.Log("PowerUpItem Added to EquipItems:- " + abilityItemData.powerUpItemName);
    }

    public bool IsPlayerHasEnoughCoins(int powerUpPrice)
    {
        return PlayerCoins >= powerUpPrice;
    }

    public SerializedDictionary<AbilityItemType, AbilityItemData> GetEquipItems()
    {
        return _playerAbilityItemData;
    }

    private void RemoveEquipItem(AbilityItemType itemType)
    {
        _playerAbilityItemData.Remove(itemType);
    }
    
    private void SetPlayerAbilityItemPurchasedStatus(AbilityItemType itemType, bool status)
    {
        if (_playerAbilityItemData.TryGetValue(itemType, out var abilityItemData))
        {
            abilityItemData.isPurchased = status;
            _playerAbilityItemData[itemType] = abilityItemData;
        }
        
        //Testing to see it's updated or not
        Debug.Log("PowerUpItem Updated:- " + abilityItemData.powerUpItemName + " IsPurchased:- " + abilityItemData.isPurchased);
    }
    
    public void SetPlayerAbilityItemEquippedStatus(AbilityItemType itemType, bool isEquipped)
    {
        if (_playerAbilityItemData.TryGetValue(itemType, out var abilityItemData))
        {
            abilityItemData.isEquipped = isEquipped;
            _playerAbilityItemData[itemType] = abilityItemData;
        }
        
        //Testing to see it's updated or not
        Debug.Log("PowerUpItem Updated:- " + abilityItemData.powerUpItemName + " IsEquipped:- " + abilityItemData.isEquipped);
    }
    
    [Rpc(SendTo.Server)]
    public void AddNetworkObjectToPlayerServerRpc(AbilityItemType itemType,Vector3 position,Quaternion rotation,RpcParams rpcParams = default)
    {
        if (abilityItemNetworkPrefabs.TryGetValue(itemType, out var abilityItemPrefab))
        {
            var networkObjectRef = Instantiate(abilityItemPrefab,position,rotation);
            var networkObject = networkObjectRef.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
            var IAbilityNetworkHandler = networkObject.GetComponent<IAbilityNetworkHandler>();
            IAbilityNetworkHandler?.HideGameObjectFromAllClients();
            Debug.Log("NetworkObject Spawned:- " + networkObject.NetworkObjectId);
        }
    }

    /// <summary>
    /// MANAGING ABILITIES OF THE PLAYER
    /// </summary>
    /// <param name="ability"></param>
    /// <typeparam name="T"></typeparam>
    #region ABILITIES

    private void AddAbility<T>(T ability) where T : IAbilityHandler
    {
        _abilities[typeof(T)] = ability;
        OnAbilityAdded?.Invoke(typeof(T));
        Debug.Log($"Ability of type {typeof(T).Name} added.");
    }

    // Add ability using a runtime type
    private void AddAbility(Type type, IAbilityHandler ability)
    {
        if (_abilities.ContainsKey(type))
        {
            Debug.LogWarning($"Ability of type {type.Name} is already added.");
            return;
        }

        _abilities[type] = ability;
        OnAbilityAdded?.Invoke(type);
        Debug.Log($"Ability of type {type.Name} added.");
    }

    // Retrieve ability using a generic type
    public T GetAbility<T>() where T : IAbilityHandler
    {
        if (_abilities.TryGetValue(typeof(T), out var abilityHandler))
        {
            Debug.Log($"Ability of type {typeof(T).Name} retrieved.");
            return (T)abilityHandler;
        }

        Debug.LogWarning($"Ability of type {typeof(T).Name} not found.");
        return default;
    }

    // Remove an ability by type
    public void RemoveAbility<T>() where T : IAbilityHandler
    {
        if (_abilities.Remove(typeof(T)))
        {
            Debug.Log($"Ability of type {typeof(T).Name} removed.");
        }
        else
        {
            Debug.LogWarning($"Ability of type {typeof(T).Name} not found for removal.");
        }
    }

    public void ListAbilities()
    {
        Debug.Log("Listing all registered abilities:");
        foreach (var ability in _abilities)
        {
            Debug.Log($"- {ability.Key.Name}");
        }
    }

    /// <summary>
    /// Function to handle the ability attachment to the player.
    /// </summary>
    /// <param name="eAbilityItemType"></param>
    /// <param name="eAbilityItemData"></param>
    public void EquipAbilityAttachmentHandler(AbilityItemType eAbilityItemType, AbilityItemData eAbilityItemData)
    {
        Debug.Log("Equipping the ability attachment to the player");
        var abilityHandlerRef = eAbilityItemData.abilityHandler;
        if (abilityHandlerRef == null)
        {
            Debug.LogError("Ability handler is null in AbilityItemData!");
            return;
        }
        
        // Instantiate the ability prefab to Player
        var abilityInstance = Instantiate(abilityHandlerRef, abilitiesHolderTransform);
        
        var abilityHandler = abilityInstance.GetComponent<IAbilityHandler>();

        if (abilityHandler == null)
        {
            Debug.LogError(
                $"Ability handler does not implement IAbilityHandler. Object: {eAbilityItemData.abilityHandler.name}");
            Destroy(abilityInstance);
            return;
        }

        // Add ability to the manager under the interface type
        AddAbility<IAbilityHandler>(abilityHandler);

        // Add ability to the manager under its specific type
        var specificType = abilityHandler.GetType(); // Retrieve the concrete type at runtime
        AddAbility(specificType, abilityHandler);

        // Initialize the handler
        abilityHandler.Initialize(eAbilityItemData);
    }

    #endregion        

            
}
