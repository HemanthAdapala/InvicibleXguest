using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private IController _keyObjectController;
    private GameUIController _gameUIController;

    // private void Start()
    // {
    //     InitializeGameUIController();
    //     InitializeKeyObjectController();
    // }

    public void TestingStart()
    {
        InitializeGameUIController();
        InitializeKeyObjectController();
    }
    
    private void InitializeKeyObjectController()
    {
        _keyObjectController = FindAnyObjectByType<KeyObjectController>();
        _keyObjectController.Initialize();
    }
    
    public KeyObjectController GetKeyObjectController()
    {
        return (KeyObjectController)_keyObjectController;
    }

    private void InitializeGameUIController()
    {
        _gameUIController = FindAnyObjectByType<GameUIController>();
    }
    public GameUIController GetGameUIController()
    {
        return _gameUIController != null ? _gameUIController : null;
    }

    
    private static void InitializeDistanceVisibilityRule()
    {
        //Initialize Visibility Rules
        var distanceVisibilityRule = new DistanceVisibilityRule(5f);
        VisibilityManager.Instance.InitializeVisibilityRules(distanceVisibilityRule);
    }
}
