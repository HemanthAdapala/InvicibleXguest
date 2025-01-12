using System;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts
{
    public class BoughtPowerUpItemHandler : MonoBehaviour
    {
        [SerializeField] private Image powerUpItemSprite = null;
        
        public void SetUiData(AbilityItemType key, AbilityItemData itemData)
        {
            powerUpItemSprite.sprite = itemData.powerUpItemSprite;       
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                PlayerManager.LocalInstance.SetPlayerAbilityItemEquippedStatus(key,true);
                PlayerManager.LocalInstance.EquipAbilityAttachmentHandler(key, itemData);
                Debug.Log("Ability Item button clicked" + key + " " + itemData.powerUpItemName);
                Destroy(gameObject);
            });
        }
    }
}