using UnityEngine;
using UnityEngine.UI;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;


    private void Awake()
    {
        newGameButton.onClick.AddListener(OnClickNewGameButton);
        musicButton.onClick.AddListener(OnClickMusicButton);
        optionsButton.onClick.AddListener(OnClickOptionsButton);
        quitButton.onClick.AddListener(OnClickQuitButton);

        Time.timeScale = 1f;
    }

    private void OnClickNewGameButton()
    {
        Debug.Log("OnClickNewGameButton");
        iXgGameMultiplayer.PlayMultiplayer = true;
        Loader.Load(Loader.Scene.LobbyScene);
    }
    
    private void OnClickMusicButton()
    {
        Debug.Log("OnClickMusicButton");
    }

    private void OnClickOptionsButton()
    {
        Debug.Log("OnClickOptionsButton");
    }
    
    private void OnClickQuitButton()
    {
        Debug.Log("OnClickQuitButton");
    }
}
