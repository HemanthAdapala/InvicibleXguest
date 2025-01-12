using System;
using UnityEngine;

public class DoorTrigger : MonoBehaviour, IPlayerSpawnListener
{
    [SerializeField] private Transform doorObject;

    private void Start()
    {
        GameEventsManager.Instance.RegisterListeners(this);
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null) GameEventsManager.Instance.UnRegisterListeners(this);
    }

    public void OnPlayerSpawned()
    {
        if (PlayerController.Instance != null) PlayerController.Instance.OnDoorInteracted += OnEnteredDoorEvent;
    }

    private void OnEnteredDoorEvent(object sender, EventArgs e)
    {
        Debug.Log("Player entered door");
        var boxCollider = doorObject.GetComponentInChildren<BoxCollider>();
        boxCollider.isTrigger = true;
        doorObject.gameObject.SetActive(false);
    }

    private void OnRoomEnteredRpcToOwnerOnBehaviour()
    {
    }
}