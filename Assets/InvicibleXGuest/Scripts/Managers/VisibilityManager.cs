using System;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    
    #region SINGLETON

    public static VisibilityManager Instance { get; private set; }

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
    
    #endregion
    
    private List<VisibilityRule> _visibilityRules = new List<VisibilityRule>();
    private readonly Dictionary<GameObject, HashSet<GameObject>> _playerVisibilityMap = new();
    
    // Method to initialize and add visibility rules
    public void InitializeVisibilityRules(params VisibilityRule[] rules)
    {
        _visibilityRules.AddRange(rules);
    }

    public void RegisterPlayers(GameObject player, List<GameObject> allPlayers)
    {
        // Initialize entry for the observer player if not present
        if (!_playerVisibilityMap.ContainsKey(player))
        {
            _playerVisibilityMap[player] = new HashSet<GameObject>();
        }

        // Initialize visibility to true for all players initially
        foreach (GameObject otherPlayer in allPlayers)
        {
            if (otherPlayer != player)
            {
                // Ensure the target player has its own entry
                if (!_playerVisibilityMap.ContainsKey(otherPlayer))
                {
                    _playerVisibilityMap[otherPlayer] = new HashSet<GameObject>();
                }

                // Set the initial visibility state (visible by default)
                SetPlayerVisibility(player, otherPlayer, true);
            }
        }
    }
    
    private void SetPlayerVisibility(GameObject observer, GameObject target, bool isVisible)
    {
        Renderer[] targetRenderers = target.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in targetRenderers)
        {
            renderer.enabled = isVisible;
        }

        // Store visibility state
        if (isVisible)
        {
            _playerVisibilityMap[observer].Add(target);
        }
        else
        {
            _playerVisibilityMap[observer].Remove(target);
        }
    }
    
    public void UpdateVisibility(GameObject observer, GameObject subject)
    {
        bool isVisible = false;
        foreach (var rule in _visibilityRules)
        {
            if (rule.CanSee(observer, subject))
            {
                isVisible = true;
                break; // If any rule passes, visibility is granted
            }
        }

        ToggleVisibility(subject, isVisible);
        if (isVisible)
        {
            if (!_playerVisibilityMap[observer].Contains(subject))
                _playerVisibilityMap[observer].Add(subject);
        }
        else
        {
            _playerVisibilityMap[observer].Remove(subject);
        }
    }

    private void ToggleVisibility(GameObject player, bool isVisible)
    {
        Renderer[] renderers = player.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
    }
}
