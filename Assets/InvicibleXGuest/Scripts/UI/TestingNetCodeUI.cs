using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI playerInGameText;
    private void Awake()
    {
        hostButton.onClick.AddListener(OnClickHostButton);
        clientButton.onClick.AddListener(OnClickClientButton);
    }

    private void Start()
    {
        //playerInGameText.SetText("Player in game: " + PlayerManager.Instance.GetPlayerInGame());
    }

    private void OnClickClientButton()
    {
        Debug.Log("Client button clicked");
        NetworkManager.Singleton.StartClient();
        GameManager.Instance.TestingStart();
        Hide(); 
    }

    private void OnClickHostButton()
    {
        Debug.Log("Host button clicked");
        NetworkManager.Singleton.StartHost();
        GameManager.Instance.TestingStart();
        Hide();
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }
}
