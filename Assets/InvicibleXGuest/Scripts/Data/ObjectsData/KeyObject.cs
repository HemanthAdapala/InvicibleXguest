using System;
using StarterAssets;
using UnityEngine;

public class KeyObject : BaseObject
{
    public bool IsPickedUp { get; private set; }
    public ulong KeyHolderId { get; private set; }
    
    public bool IsHeld { get; private set; } = false;

    public void SetHeldState(bool held)
    {
        IsHeld = held;
    }

    private void Start()
    {
        if(PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnKeyObjectPickedUp += OnKeyObjectPickedUp;
        }
        InitializeBaseData();
    }

    private void InitializeBaseData()
    {
        objectType = ObjectType.Key;
        IsPickedUp = false;
        KeyHolderId = 0;
    }

    private void OnKeyObjectPickedUp(object sender, PlayerController.OnKeyObjectPickedUpEventArgs e)
    {
        IsPickedUp = true;
        KeyHolderId = e.keyHolderId;
    }
}