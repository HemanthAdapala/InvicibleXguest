using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetCodeUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        hostButton.onClick.AddListener(OnClickHostButton);
        clientButton.onClick.AddListener(OnClickClientButton);
    }

    private void OnClickClientButton()
    {
        Debug.Log("Client button clicked");
        NetworkManager.Singleton.StartClient();
        Hide();
    }

    private void OnClickHostButton()
    {
        Debug.Log("Host button clicked");
        NetworkManager.Singleton.StartHost();
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}