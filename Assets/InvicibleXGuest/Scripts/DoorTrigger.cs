using System;
using StarterAssets;
using UnityEngine;

public class DoorTrigger : MonoBehaviour,IPlayerSpawnListener
{
    [SerializeField] private Transform doorObject;
    
    private void Start()
    {
        GameEventsManager.Instance.RegisterListeners(this);
    }

    public void OnPlayerSpawned()
    {
        if(PlayerController.LocalInstance != null)
        {
            PlayerController.LocalInstance.OnDoorEnterEvent += OnEnteredDoorEvent;
        }
    }

    private void OnEnteredDoorEvent(object sender, EventArgs e)
    {
        Debug.Log("Player entered door");
        if(PlayerController.LocalInstance.HasKey())
        {
            var boxCollider = doorObject.GetComponentInChildren<BoxCollider>();
            boxCollider.isTrigger = true;
            doorObject.gameObject.SetActive(false);
        }
        else
        {
            return;
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.UnRegisterListeners(this);
        }
    }
}
