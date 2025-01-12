using System;
using InvicibleXGuest.Scripts.Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts.UI
{
    public class LobbyMessageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button closeButton;


        private void Awake()
        {
            closeButton.onClick.AddListener(Hide);
        }

        private void Start()
        {
            iXgGameMultiplayer.Instance.OnFailedToJoin += iXgGameMultiplayer_OnFailedToJoinGame;
            iXgGameLobby.Instance.OnCreateLobbyStarted += iXgGameLobby_OnCreateLobbyStarted;
            iXgGameLobby.Instance.OnCreateLobbyFailed += iXgGameLobby_OnCreateLobbyFailed;
            iXgGameLobby.Instance.OnJoinStarted += iXgGameLobby_OnJoinStarted;
            iXgGameLobby.Instance.OnJoinFailed += iXgGameLobby_OnJoinFailed;
            iXgGameLobby.Instance.OnQuickJoinFailed += iXgGameLobby_OnQuickJoinFailed;

            Hide();
        }

        private void OnDestroy()
        {
            iXgGameMultiplayer.Instance.OnFailedToJoin -= iXgGameMultiplayer_OnFailedToJoinGame;
            iXgGameLobby.Instance.OnCreateLobbyStarted -= iXgGameLobby_OnCreateLobbyStarted;
            iXgGameLobby.Instance.OnCreateLobbyFailed -= iXgGameLobby_OnCreateLobbyFailed;
            iXgGameLobby.Instance.OnJoinStarted -= iXgGameLobby_OnJoinStarted;
            iXgGameLobby.Instance.OnJoinFailed -= iXgGameLobby_OnJoinFailed;
            iXgGameLobby.Instance.OnQuickJoinFailed -= iXgGameLobby_OnQuickJoinFailed;
        }

        private void iXgGameLobby_OnQuickJoinFailed(object sender, EventArgs e)
        {
            ShowMessage("Could not find a Lobby to Quick Join!");
        }

        private void iXgGameLobby_OnJoinFailed(object sender, EventArgs e)
        {
            ShowMessage("Failed to join Lobby!");
        }

        private void iXgGameLobby_OnJoinStarted(object sender, EventArgs e)
        {
            ShowMessage("Joining Lobby...");
        }

        private void iXgGameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
        {
            ShowMessage("Failed to create Lobby!");
        }

        private void iXgGameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
        {
            ShowMessage("Creating Lobby...");
        }

        private void iXgGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
        {
            if (NetworkManager.Singleton.DisconnectReason == "")
                ShowMessage("Failed to connect");
            else
                ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }

        private void ShowMessage(string message)
        {
            Show();
            messageText.text = message;
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
}