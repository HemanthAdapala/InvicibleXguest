using System;
using InvicibleXGuest.Scripts;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class KeyObjectController : NetworkBehaviour, IController
{
    [Header("Random Position Settings")]
    [SerializeField] public Vector3 minBounds;
    [SerializeField] public Vector3 maxBounds;
    [SerializeField] public Transform keyPositionSpawnObjectParent;
    [SerializeField] private RandomPositionGenerator randomPositionGenerator;
    [SerializeField] private Transform keyPrefabObject;

    private float spawnAfterSeconds = 10f; // Time to wait before spawning
    private bool isInitialized = false;
    private bool hasSpawned = false; // Ensures the object spawns only once

    public event EventHandler<CountDownStateUpdatedEventArgs> OnCountDownStateUpdated;

    public class CountDownStateUpdatedEventArgs : EventArgs
    {
        public CountDownState CountDownState;
    }

    public void Initialize()
    {
        Debug.Log("KeyObjectController Initialized");
        GameUIController.Instance.UpdatedKeyObjectStatus(1); // Notify UI
        isInitialized = true;

        NotifyCountDownState(CountDownState.Started);
    }

    private void Update()
    {
        if (!isInitialized || hasSpawned) return; // Skip if already spawned or not initialized

        spawnAfterSeconds -= Time.deltaTime;

        if (spawnAfterSeconds <= 0f)
        {
            hasSpawned = true; // Mark as spawned
            if (IsServer)
            {
                //SpawnKeyObjectServerRpc();
                SpawnKeyObjectRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnKeyObjectRpc()
    {
        SpawnKeyObject();
        NotifyToAllClientsAboutKeySpawnedRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void NotifyToAllClientsAboutKeySpawnedRpc()
    {
        GameUIController.Instance.UpdatedKeyObjectStatus(2);
    }

    private void SpawnKeyObject()
    {
        Debug.Log("Spawning KeyObject...");
        var spawnPosition = randomPositionGenerator.GetRandomPositionWithinBounds(minBounds, maxBounds);

        // Instantiate the KeyObject prefab
        var instance = Instantiate(keyPrefabObject, spawnPosition, Quaternion.identity, keyPositionSpawnObjectParent);
        var networkObject = instance.GetComponent<NetworkObject>();

        // Ensure the NetworkObject exists before spawning
        if (networkObject != null)
        {
            networkObject.Spawn();
            NotifyCountDownState(CountDownState.Finished);

            // Initialize the KeyObject
            var keyObject = instance.GetComponent<KeyObject>();
            if (keyObject != null)
            {
                keyObject.Initialize(1, ObjectType.Key);
            }
        }
        else
        {
            Debug.LogError("KeyObject prefab is missing a NetworkObject component!");
        }
    }

    private void NotifyCountDownState(CountDownState state)
    {
        OnCountDownStateUpdated?.Invoke(this, new CountDownStateUpdatedEventArgs { CountDownState = state });
    }
    
    
    public void SetKeyHolderData(ulong currentKeyObjectId)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(currentKeyObjectId,out var networkObjectKeyObject))
        {
            var keyObject = networkObjectKeyObject.GetComponent<KeyObject>();
            if (keyObject != null)
            {
                var clientId = Unity.Netcode.NetworkManager.Singleton.LocalClientId;
                if(Unity.Netcode.NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
                {
                    var networkObject = networkClient.PlayerObject.GetComponent<NetworkObject>();
                    if (networkObject != null)
                    {
                        PlayerController.LocalInstance.CurrentKeyObject = keyObject;
                    }
                    
                }
            }
        }
    }
}

public enum CountDownState
{
    NotStarted,
    Started,
    Finished
}

