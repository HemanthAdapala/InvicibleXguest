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
        
    }

    

    #endregion

    
}
