using InvicibleXGuest.Scripts.Managers;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            iXgGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        readyButton.onClick.AddListener(() => { CharacterSelectReady.Instance.SetPlayerReady(); });
    }

    private void Start()
    {
        var lobby = iXgGameLobby.Instance.GetLobby();
        lobbyNameText.text = "Lobby Name:- " + lobby.Name;
        lobbyCodeText.text = "Lobby Code:- " + lobby.LobbyCode;
    }
}