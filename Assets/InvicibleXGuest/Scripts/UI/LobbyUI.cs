using System.Collections.Generic;
using InvicibleXGuest.Scripts.Managers;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private Button joinCodeButton;
        [SerializeField] private TMP_InputField joinCodeInputField;
        [SerializeField] private TMP_InputField playerNameInputField;

        [SerializeField] private LobbyCreateUI lobbyCreateUI;
        [SerializeField] private Transform lobbyContainer;
        [SerializeField] private Transform lobbyTemplate;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                iXgGameLobby.Instance.LeaveLobby();
                Loader.Load(Loader.Scene.MainMenuScene);
            });

            createLobbyButton.onClick.AddListener(() => { lobbyCreateUI.Show(); });

            quickJoinButton.onClick.AddListener(() => { iXgGameLobby.Instance.QuickJoin(); });

            joinCodeButton.onClick.AddListener(() => { iXgGameLobby.Instance.JoinWithCode(joinCodeInputField.text); });

            playerNameInputField.onEndEdit.AddListener(value => { iXgGameMultiplayer.Instance.SetPlayerName(value); });
        }


        private void Start()
        {
            var playerName = iXgGameMultiplayer.Instance.GetPlayerName();
            iXgGameMultiplayer.Instance.SetPlayerName(playerName);
            playerNameInputField.text = playerName;
            UpdateLobbyList(new List<Lobby>());
            iXgGameLobby.Instance.OnLobbyListChanged += iXgGameLobby_OnLobbyListChanged;
        }

        private void OnDestroy()
        {
            iXgGameLobby.Instance.OnLobbyListChanged -= iXgGameLobby_OnLobbyListChanged;
        }

        private void iXgGameLobby_OnLobbyListChanged(object sender, iXgGameLobby.OnLobbyListChangedEventArgs e)
        {
            UpdateLobbyList(e.lobbyList);
        }

        private void UpdateLobbyList(List<Lobby> lobbyList)
        {
            foreach (Transform child in lobbyContainer)
            {
                if (child == lobbyTemplate) continue;
                Destroy(child.gameObject);
            }

            foreach (var lobby in lobbyList)
            {
                var lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
            }
        }
    }
}