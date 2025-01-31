using System;
using InvicibleXGuest.Scripts;
using InvicibleXGuest.Scripts.Interfaces;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour, IBaseGameUI
{
    [SerializeField] private TextMeshProUGUI keyObjectUpdater;
    
    [SerializeField] private Transform boughtPowerUpItemParent;
    [SerializeField] private Transform availablePowerUpItemPrefab;

    private void Awake()
    {
        Show();
        KeyObjectController.Instance.OnKeyObjectSpawnedEvent += KeyObjectController_OnKeyObjectSpawned;
        KeyObjectController.Instance.OnKeyObjectPickedUpEvent += KeyObjectController_OnKeyObjectPickedUp;
        KeyObjectController.Instance.OnKeyObjectDroppedEvent += KeyObjectController_OnKeyObjectDropped;
    }

    private void OnDestroy()
    {
        KeyObjectController.Instance.OnKeyObjectSpawnedEvent -= KeyObjectController_OnKeyObjectSpawned;
        KeyObjectController.Instance.OnKeyObjectPickedUpEvent -= KeyObjectController_OnKeyObjectPickedUp;
        KeyObjectController.Instance.OnKeyObjectDroppedEvent -= KeyObjectController_OnKeyObjectDropped;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void KeyObjectController_OnKeyObjectSpawned(object sender, EventArgs e)
    {
        keyObjectUpdater.SetText("Key Spawned");
    }

    private void KeyObjectController_OnKeyObjectPickedUp(object sender, EventArgs e)
    {
        keyObjectUpdater.SetText("Key Picked Up");
    }

    private void KeyObjectController_OnKeyObjectDropped(object sender, EventArgs e)
    {
        keyObjectUpdater.SetText("Key Dropped");
    }
    
    private void OnPurchasedAbilityItems(object sender, SupportItemsShopUI.OnPowerUpItemBoughtEventArgs e)
    {
        var boughtPowerUpItemTransform = Instantiate(availablePowerUpItemPrefab, boughtPowerUpItemParent);
        var boughtPowerUpItemHandler = boughtPowerUpItemTransform.GetComponent<BoughtPowerUpItemHandler>();
        boughtPowerUpItemHandler.SetUiData(e.ItemType, e.ItemData);
    }
}