using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerManager : NetworkBehaviour
{
    
    #region SINGLETON
    public static PlayerManager Instance { get;private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    #endregion

    private NetworkVariable<int> playerInGame = new NetworkVariable<int>();
    
    public int GetPlayerInGame()
    {
        return playerInGame.Value;
    }

    #region SERIALIZE_FIELD
    
    [SerializeField] public GameObject playerPrefab;
    [SerializeField] public CinemachineVirtualCamera cinemachineVirtualCamera;

    #endregion

    #region PUBLIC_FIELDS

    #endregion
    
    #region PRIVATE_FIELDS

    private Player _mainPlayer;
    
    #endregion

    #region UNITY_FUNCTIONS

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (playerID) =>
        {
            if (IsServer)
            {
                playerInGame.Value++;
            }
        };
        
        NetworkManager.Singleton.OnClientDisconnectCallback += (playerID) =>
        {
            if (IsServer)
            {
                playerInGame.Value--;
            }
        };
    }

    #endregion

    #region PUBLIC_FUNCTIONS

    public Player GetPlayer()
    {
        return _mainPlayer;
    }
    
    public void SetPlayer(Player playerComponent)
    {
        _mainPlayer = playerComponent;
    }

    #endregion

    #region PRIVATE_FUNCTIONS

    #endregion

    
}
