using System;
using InvicibleXGuest.Scripts.Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private GameObject playerNameText;
    [SerializeField] private PlayerVisual _playerVisual;
    [SerializeField] private Button kickButton;


    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            var playerData = iXgGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            Debug.Log("Kicking player: " + playerData.clientId);
            iXgGameLobby.Instance.KickPlayerFromLobby(playerData.playerId.ToString());
            iXgGameMultiplayer.Instance.KickPlayerFromLobby(playerData.clientId);
        });
    }

    private void Start()
    {
        iXgGameMultiplayer.Instance.OnNetworkPlayerListChangedEvent += OnNetworkPlayerListChangedEvent;
        CharacterSelectReady.Instance.OnReadyChangedEvent += CharacterSelectReady_OnReadyChangedEvent;
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        iXgGameMultiplayer.Instance.OnNetworkPlayerListChangedEvent -= OnNetworkPlayerListChangedEvent;
        CharacterSelectReady.Instance.OnReadyChangedEvent -= CharacterSelectReady_OnReadyChangedEvent;
    }

    private void CharacterSelectReady_OnReadyChangedEvent(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void OnNetworkPlayerListChangedEvent(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (iXgGameMultiplayer.Instance.IsPlayerConnected(playerIndex))
        {
            Show();
            var playerData = iXgGameMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            readyText.gameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerNameText.GetComponent<TextMeshPro>().text = playerData.playerName.ToString();
            _playerVisual.SetPlayerColor(iXgGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}