using System;
using InvicibleXGuest.Scripts;
using Unity.Netcode;
using UnityEngine;

public class KeyObjectController : NetworkBehaviour, IController
{
    [Header("Random Position Settings")]
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private Transform keyPositionSpawnObjectParent;
    [SerializeField] private RandomPositionGenerator randomPositionGenerator;
    [SerializeField] private Transform keyPrefabObject;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 10f;

    private NetworkVariable<KeyObjectData> _keyObjectData;
    private float _spawnTimer;
    private bool _hasSpawned;
    private bool _isInitialized;

    public static KeyObjectController Instance { get; private set; }

    public event EventHandler OnKeyObjectSpawnedEvent;
    public event EventHandler OnKeyObjectPickedUpEvent;
    public event EventHandler OnKeyObjectDroppedEvent;
    public event EventHandler<CountDownStateUpdatedEventArgs> OnCountDownStateUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _keyObjectData = new NetworkVariable<KeyObjectData>();
        _keyObjectData.OnValueChanged += KeyObjectController_OnKeyObjectDataChanged;
        Initialize();
    }

    private void KeyObjectController_OnKeyObjectDataChanged(KeyObjectData previousvalue, KeyObjectData newvalue)
    {
        Debug.Log("KeyObjectData changed: " + previousvalue.ownerId + " " + previousvalue.isHeld + " -> " + newvalue.ownerId + " " + newvalue.ownerId);
    }

    private void Update()
    {
        if (_isInitialized && !_hasSpawned)
        {
            UpdateSpawnTimer();
        }
    }

    public void Initialize()
    {
        Debug.Log("KeyObjectController Initialized");
        _isInitialized = true;
        _spawnTimer = spawnDelay;
        NotifyCountDownState(CountDownState.Started);
    }

    private void UpdateSpawnTimer()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f)
        {
            _hasSpawned = true;
            if (IsServer)
            {
                SpawnKeyObjectServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKeyObjectServerRpc()
    {
        SpawnKeyObject();
        NotifyKeyObjectSpawnedClientRpc();
    }

    [ClientRpc]
    private void NotifyKeyObjectSpawnedClientRpc()
    {
        OnKeyObjectSpawnedEvent?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnKeyObject()
    {
        Debug.Log("Spawning KeyObject...");

        var spawnPosition = randomPositionGenerator.GetRandomPositionWithinBounds(minBounds, maxBounds);
        var instance = Instantiate(keyPrefabObject, spawnPosition, Quaternion.identity, keyPositionSpawnObjectParent);

        if (TrySpawnNetworkObject(instance))
        {
            InitializeKeyObject(instance);
            NotifyCountDownState(CountDownState.Finished);
            Debug.Log("KeyObject spawned successfully!");
        }
        else
        {
            Debug.LogError("KeyObject prefab is missing a NetworkObject component!");
        }
    }
    
    private void InitializeKeyObject(Transform instance)
    {
        var keyObject = instance.GetComponent<KeyObject>();
        if (keyObject != null)
        {
            keyObject.Initialize(false);
            keyObject.RotateKeyObjectClientRpc();
        }
    }

    private bool TrySpawnNetworkObject(Transform instance)
    {
        var networkObject = instance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
            if (IsServer)
            {
                // Initialize key object data for NetworkVariable at spawn
                _keyObjectData.Value = new KeyObjectData
                {
                    isHeld = false,ownerId = 100
                };
            }
            return true;
        }
        return false;
    }

    

    private void NotifyCountDownState(CountDownState state)
    {
        OnCountDownStateUpdated?.Invoke(this, new CountDownStateUpdatedEventArgs { CountDownState = state });
    }

    public void PickUpKeyObject(KeyObject keyObject)
    {
        if (keyObject.IsHeld) return;

        RequestPickupKeyObjectServerRpc(keyObject.NetworkObjectId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestPickupKeyObjectServerRpc(ulong keyObjectId, ServerRpcParams rpcParams = default)
    {
        var senderClientId = rpcParams.Receive.SenderClientId;
        //Change the variable data of the KeyObject(False) - when picked up
        ChangeKeyObjectVariableData(senderClientId,true);
        RequestPickUpKeyObjectClientRpc(keyObjectId, senderClientId);
    }

    public void ChangeKeyObjectVariableData(ulong senderClientId,bool isHeld)
    {
        if (IsServer)
        {
            // Update key object data for NetworkVariable
            _keyObjectData.Value = new KeyObjectData
            {
                isHeld = isHeld, ownerId = true ? senderClientId : 100
            };
        }
    }

    [ClientRpc]
    private void RequestPickUpKeyObjectClientRpc(ulong keyObjectId, ulong senderClientId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(keyObjectId, out var keyNetworkObject))
            return;
        
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(senderClientId, out var networkClient))
        {
            Debug.LogWarning($"Sender client with ID {senderClientId} not found.");
            return;
        }
        var playerController = networkClient.PlayerObject.GetComponent<PlayerController>();
        playerController.currentKeyObject = keyNetworkObject.GetComponent<KeyObject>();
        
        //SetKeyObjectData & follow target
        var keyObject = keyNetworkObject.GetComponent<KeyObject>();
        keyObject.SetHeldState(true);
        var followTarget = keyObject.GetComponent<FollowTransform>();
        followTarget.FollowTarget(playerController.transform);
        keyObject.Initialize(true);
        
        //Notify all clients that KeyObject data is changed and someone is pickedUp
        NotifyToAllClients();
    }
    
    private void NotifyToAllClients()
    {
        Debug.Log("Notifying all clients...");
        OnKeyObjectPickedUpEvent?.Invoke(this, EventArgs.Empty);
    }
    
    public void NotifyKeyObjectDropped(ulong keyObjectId, Vector3 dropPosition)
    {
        NotifyKeyObjectDroppedClientRpc(keyObjectId, dropPosition);
    }

    [ClientRpc]
    private void NotifyKeyObjectDroppedClientRpc(ulong keyObjectId, Vector3 dropPosition)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(keyObjectId, out var keyNetworkObject))
        {
            var keyObject = keyNetworkObject.GetComponent<KeyObject>();
            var followTransform = keyObject.GetComponent<FollowTransform>();
            followTransform?.StopFollowing();

            keyObject.transform.SetParent(null);
            keyObject.transform.position = dropPosition;
            keyObject.SetHeldState(false);
            OnKeyObjectDroppedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
     
     

    public class CountDownStateUpdatedEventArgs : EventArgs
    {
        public CountDownState CountDownState;
    }

    public void OnKeyObjectDropped()
    {
        OnKeyObjectDroppedEvent?.Invoke(this, EventArgs.Empty);
    }
}

public enum CountDownState
{
    NotStarted,
    Started,
    Finished
}
