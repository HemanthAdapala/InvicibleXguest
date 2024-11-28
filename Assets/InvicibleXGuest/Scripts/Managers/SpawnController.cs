using System.Collections.Generic;
using InvicibleXGuest.Scripts;
using UnityEngine;

public class SpawnController : MonoBehaviour,IController
{
    
    
    #region PRIVATE_FIELDS
    
    private int _playersMax;

    #endregion

    #region SERIALIZE_FIELD
    
    [SerializeField] private List<Vector3> playerPositions;
    
    [SerializeField] private GameObject player;

    #endregion

    #region PUBLIC_FIELDS

    #endregion
    
    public void Initialize()
    {
        Debug.Log("SpawnController Initialized");
        _playersMax = 1;
    }

    #region PUBLIC_FUNCTIONS
    
    public void SpawnPlayers()
    {
        for (int i = 0; i < _playersMax; i++)
        {
            var playerInstance = Instantiate(player, playerPositions[i], Quaternion.identity);
            //If you want the camera to follow only the first player for testing
            if (i == 0)
            {
                var playerComponent = playerInstance.GetComponent<Player>();
                //playerComponent.PlayerCameraFollow();
                PlayerManager.Instance.SetPlayer(playerComponent);
            }
        }
    }

    

    #endregion

    
}
