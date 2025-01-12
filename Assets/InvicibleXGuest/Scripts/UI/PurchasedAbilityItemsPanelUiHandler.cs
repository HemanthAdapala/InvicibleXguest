using UnityEngine;

namespace InvicibleXGuest.Scripts.UI
{
    public class PurchasedAbilityItemsPanelUiHandler : MonoBehaviour
    {
        [SerializeField] private Transform boughtPowerUpItemParent;
        [SerializeField] private Transform availablePowerUpItemPrefab;

        private void Start()
        {
        }

        private void OnPurchasedAbilityItems(object sender, AbilityItemsShopPanelHandler.OnPowerUpItemBoughtEventArgs e)
        {
            var boughtPowerUpItemTransform = Instantiate(availablePowerUpItemPrefab, boughtPowerUpItemParent);
            var boughtPowerUpItemHandler = boughtPowerUpItemTransform.GetComponent<BoughtPowerUpItemHandler>();
            boughtPowerUpItemHandler.SetUiData(e.AbilityItemType, e.AbilityItemData);
        }
    }
}