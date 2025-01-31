using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIToolKit : MonoBehaviour
{
    private UIDocument _uiDocument;
    
    //BUTTONS
    private Button _playButton;
    private Button _settingsButton;
    private Button _exitButton;
    private Button _displaySettingsButton;
    private Button _audioSettingsButton;
    
    
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        InitializeButtons();
    }

    private void OnEnable()
    {
        RegisterListeners();
    }

    private void OnDisable()
    {
        UnRegisterListeners();
    }

    private void InitializeButtons()
    {
        _playButton = _uiDocument.rootVisualElement.Q<Button>("PlayButton");
        _settingsButton = _uiDocument.rootVisualElement.Q<Button>("SettingsButton");
        _exitButton = _uiDocument.rootVisualElement.Q<Button>("ExitButton");
        _displaySettingsButton = _uiDocument.rootVisualElement.Q<Button>("DisplaySettingsButton");
        _audioSettingsButton = _uiDocument.rootVisualElement.Q<Button>("AudioSettingsButton");
    }
    
    private void RegisterListeners()
    {
        _playButton.RegisterCallback<ClickEvent>(OnClickPlayButton);
        _settingsButton.RegisterCallback<ClickEvent>(OnClickSettingsButton);
        _exitButton.RegisterCallback<ClickEvent>(OnClickExitButton);
        _displaySettingsButton.RegisterCallback<ClickEvent>(OnClickDisplaySettingsButton);
        _audioSettingsButton.RegisterCallback<ClickEvent>(OnClickAudioSettingsButton);
    }

    private void OnClickPlayButton(ClickEvent evt)
    {
        Debug.Log("OnClickNewGameButton");
        iXgGameMultiplayer.PlayMultiplayer = true;
        Loader.Load(Loader.Scene.LobbyScene);
    }
    
    private void OnClickSettingsButton(ClickEvent evt)
    {
        Debug.Log("OnClickOptionsButton");
    }
    
    private void OnClickExitButton(ClickEvent evt)
    {
        Debug.Log("OnClickQuitButton");
    }
    
    private void OnClickDisplaySettingsButton(ClickEvent evt)
    {
        Debug.Log("OnClickDisplaySettingsButton");
    }
    
    private void OnClickAudioSettingsButton(ClickEvent evt)
    {
        Debug.Log("OnClickAudioSettingsButton");  
    }

    private void UnRegisterListeners()
    {
        _playButton.UnregisterCallback<ClickEvent>(OnClickPlayButton);
        _settingsButton.UnregisterCallback<ClickEvent>(OnClickSettingsButton);
        _exitButton.UnregisterCallback<ClickEvent>(OnClickExitButton);
        _displaySettingsButton.UnregisterCallback<ClickEvent>(OnClickDisplaySettingsButton);
        _audioSettingsButton.UnregisterCallback<ClickEvent>(OnClickAudioSettingsButton);
    }    
}
