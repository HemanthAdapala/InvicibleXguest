using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class iXgGameManager : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;
    public static iXgGameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }

        Debug.Log("Scene Loaded and Players Spawned from iXgGameManager");
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        //Pausing Game functionality
    }

    private static void InitializeDistanceVisibilityRule()
    {
        //Initialize Visibility Rules
        var distanceVisibilityRule = new DistanceVisibilityRule(5f);
        VisibilityManager.Instance.InitializeVisibilityRules(distanceVisibilityRule);
    }
}