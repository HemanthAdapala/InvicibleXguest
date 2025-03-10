using System.Collections.Generic;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    private readonly List<IPlayerSpawnListener> _playerSpawnListeners = new();
    public static GameEventsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterListeners(IPlayerSpawnListener listener)
    {
        if (!_playerSpawnListeners.Contains(listener)) _playerSpawnListeners.Add(listener);
    }

    public void UnRegisterListeners(IPlayerSpawnListener listener)
    {
        if (_playerSpawnListeners.Contains(listener)) _playerSpawnListeners.Remove(listener);
    }

    public void OnListenEvents()
    {
        foreach (var listener in _playerSpawnListeners) listener.OnPlayerSpawned();
    }
}