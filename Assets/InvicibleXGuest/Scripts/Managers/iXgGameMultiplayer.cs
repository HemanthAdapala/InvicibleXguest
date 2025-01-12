using System;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class iXgGameMultiplayer : NetworkBehaviour
{
    public const int MaxPlayers = 4;
    private const string PlayerPrefsPlayerName = "PlayerNameMulitplayer";
    public static bool PlayMultiplayer = true;

    [SerializeField] private Color[] playerColors;

    private NetworkList<PlayerData> _playerDataNetworkList;

    public static iXgGameMultiplayer Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnListChanged;
    }

    private void Start()
    {
        if (!PlayMultiplayer)
        {
            //Single Player
            //TODO: IN FUTURE
        }
    }


    public event EventHandler OnTryingToJoin;
    public event EventHandler OnFailedToJoin;
    public event EventHandler OnNetworkPlayerListChangedEvent;


    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallBack;
        // NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallBack;
        // NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_Server_OnConnectionEvent;

        NetworkManager.Singleton.StartHost();
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_Server_OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        Debug.Log("Connection Event: " + connectionEventData.EventType);
        Debug.Log("Client Id: " + connectionEventData.ClientId);
        if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
        {
            _playerDataNetworkList.Add(new PlayerData
            {
                clientId = connectionEventData.ClientId,
                colorId = GetFirstUnusedColorId()
            });
        }
        else if(connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            for (var i = 0; i < _playerDataNetworkList.Count; i++)
            {
                var playerData = _playerDataNetworkList[i];
                if (playerData.clientId == connectionEventData.ClientId) _playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    // private void NetworkManager_Server_OnClientConnectedCallBack(ulong clientId)
    // {
    //     _playerDataNetworkList.Add(new PlayerData
    //     {
    //         clientId = clientId,
    //         colorId = GetFirstUnusedColorId()
    //     });
    // }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (var i = 0; i < _playerDataNetworkList.Count; i++)
        {
            var playerData = _playerDataNetworkList[i];
            if (playerData.clientId == clientId) _playerDataNetworkList.RemoveAt(i);
        }
    }

    public void StartClient()
    {
        OnTryingToJoin?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.StartClient();
        //NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_Client_OnConnectionEvent;
    }

    private void NetworkManager_Client_OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        Debug.Log("Connection Event: " + connectionEventData.EventType);
        Debug.Log("Client Id: " + connectionEventData.ClientId);
        if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
        {
            SetPlayerNameServerRpc(GetPlayerName());
            SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
        }
        else if(connectionEventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            OnFailedToJoin?.Invoke(this, EventArgs.Empty);
        }
    }

    // private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    // {
    //     SetPlayerNameServerRpc(GetPlayerName());
    //     SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    // }
    //
    // private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    // {
    //     OnFailedToJoin?.Invoke(this, EventArgs.Empty);
    // }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        var playerData = _playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        var playerData = _playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }


    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerPrefsPlayerName, "Player " + Random.Range(0, 1000));
    }

    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString(PlayerPrefsPlayerName, playerName);
    }


    private void PlayerDataNetworkListOnListChanged(NetworkListEvent<PlayerData> changeevent)
    {
        OnNetworkPlayerListChangedEvent?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_ConnectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game is already in progress";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Game is Already full";
            return;
        }

        response.Approved = true;
    }


    public bool IsPlayerConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromIndex(int index)
    {
        return _playerDataNetworkList[index];
    }


    public Color GetPlayerColor(int colorId)
    {
        return playerColors[colorId];
    }


    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in _playerDataNetworkList)
            if (playerData.clientId == clientId)
                return playerData;

        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (var i = 0; i < _playerDataNetworkList.Count; i++)
            if (_playerDataNetworkList[i].clientId == clientId)
                return i;

        return -1;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }


    public void ChangePlayerColor(int colorId)
    {
        ChangerPlayerColorServerRpc(colorId);
    }

    //TODO:- This is how you have to modify the data
    [ServerRpc(RequireOwnership = false)]
    private void ChangerPlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
            //Color not available
            return;

        var playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        var playerData = _playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in _playerDataNetworkList)
            if (playerData.colorId == colorId)
                return false;

        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (var i = 0; i < playerColors.Length; i++)
            if (IsColorAvailable(i))
                return i;

        return -1;
    }

    public void KickPlayerFromLobby(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        //NetworkManager_Server_OnClientDisconnectCallback(clientId);
        NetworkManager_Server_OnConnectionEvent(NetworkManager.Singleton, new ConnectionEventData
        {
            EventType = ConnectionEvent.ClientDisconnected,
            ClientId = clientId
        });
    }
}