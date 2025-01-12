using InvicibleXGuest.Scripts.Managers;
using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (iXgGameMultiplayer.Instance != null)
        {
            Destroy(iXgGameMultiplayer.Instance.gameObject);
        }

        if (iXgGameLobby.Instance != null)
        {
            Destroy(iXgGameLobby.Instance.gameObject);
        }
    }
}
