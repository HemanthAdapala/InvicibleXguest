using System;
using System.Collections.Generic;
using InvicibleXGuest.Scripts.Managers;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> playerReadyDictionary;


    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        var allPlayersReady = true;

        foreach (var clientID in NetworkManager.Singleton.ConnectedClientsIds)
            if (!playerReadyDictionary.ContainsKey(clientID) || !playerReadyDictionary[clientID])
            {
                allPlayersReady = false;
                break;
            }

        if (allPlayersReady)
        {
            iXgGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.MainGameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChangedEvent?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

    #region SINGLETON

    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChangedEvent;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    #endregion
}