using UnityEngine;

public abstract class PlayerSpawnerListener : MonoBehaviour , IPlayerSpawnListener
{
    public void OnPlayerSpawned()
    {
        throw new System.NotImplementedException();
    }
}
