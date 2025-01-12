using InvicibleXGuest.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts.UI
{
    public class LobbyCreateUI : MonoBehaviour
    {
        [SerializeField] private Button createPrivateButton;
        [SerializeField] private Button createPublicButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_InputField lobbyNameInputField;


        private void Awake()
        {
            createPrivateButton.onClick.AddListener(() =>
            {
                iXgGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
            });

            createPublicButton.onClick.AddListener(() =>
            {
                iXgGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
            });

            closeButton.onClick.AddListener(Hide);
        }

        private void Start()
        {
            Hide();
        }


        public void Show()
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
            lobbyNameInputField.text = UnityEngine.Random.Range(1000, 9999).ToString();
        }

        private void Hide()
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
    }
}