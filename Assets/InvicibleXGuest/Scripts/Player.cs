using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Store the list of all players
    private List<GameObject> _allPlayers;
    private Vector3 _lastPosition;
    private readonly float _updateInterval = 0.5f; // Check every half second
    private float _nextUpdateTime = 0f;

    private BaseObject _currentHitObject;
    private const float TargetDistance = 0.5f;
    private CharacterController controller;
    private float _turnSmoothVelocity;
    private float _turnSmoothTime = 0.1f;
    private float _speed = 5f;

    

    private void Start()
    {
        
        // Initialize the list of all players
        _allPlayers = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
        // Register this player with the Visibility Manager
        VisibilityManager.Instance.RegisterPlayers(gameObject, _allPlayers);
        
        // Initialize the last known position
        _lastPosition = transform.position;
    }

    private void Update()
    {
        //Perform visibility updates at set intervals
         if (Time.time >= _nextUpdateTime)
         {
             _nextUpdateTime = Time.time + _updateInterval;
         
             // Update visibility only if the player has moved significantly
             if (Vector3.Distance(_lastPosition, transform.position) > 0.1f)
             {
                 // Update visibility for all players from the perspective of this player
                 foreach (GameObject player in _allPlayers)
                 {
                     if (player != gameObject)
                     {
                         VisibilityManager.Instance.UpdateVisibility(gameObject, player);
                     }
                 }
         
                 // Update the last position
                 _lastPosition = transform.position;
             }
         }
    }
}