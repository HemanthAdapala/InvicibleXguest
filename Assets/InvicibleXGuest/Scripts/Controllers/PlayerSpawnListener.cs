using UnityEngine;

public class PlayerSpawnerListener : MonoBehaviour, IPlayerSpawnListener
{
    public void OnPlayerSpawned()
    {
        Debug.Log("Player has spawned.");
    }
}