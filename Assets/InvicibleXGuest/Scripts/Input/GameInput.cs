using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
