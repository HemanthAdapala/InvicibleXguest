using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts
{
    public class AbilityItemDataHandler : MonoBehaviour
    {
        [SerializeField] private Image powerUpItemSprite = null;
        [SerializeField] private TextMeshProUGUI powerUpItemName = null;
        [SerializeField] private TextMeshProUGUI powerUpItemPrice = null;
        [SerializeField] private Button powerUpItemButton = null;
        [SerializeField] private Color[] color;
        [SerializeField] private Image powerUpItemBackground;


        public AbilityItemType AbilityItemType { get;private set; }
        public AbilityItemData AbilityItemData{get;private set;}
        
        public event EventHandler<OnPowerUpItemButtonClickedEventArgs> OnPowerUpItemButtonClicked;
        public class  OnPowerUpItemButtonClickedEventArgs : EventArgs
        {
           public AbilityItemType AbilityItemType;
           public AbilityItemData AbilityItemData;
        }


        public void SetData(AbilityItemType key, AbilityItemData itemData)
        {
            Button button = powerUpItemButton.GetComponent<Button>();
            if (PlayerManager.LocalInstance.IsPlayerHasEnoughCoins(AbilityItemData.powerUpItemPrice))
            {
                powerUpItemBackground.color = color[1];
                button.interactable = true;
            }
            else
            {
                powerUpItemBackground.color = color[0];
                button.interactable = false;
            }
            
            SetItemDataUI(key, itemData);
            powerUpItemButton.onClick.AddListener(OnClickPowerUpButton);
        }

        public void SetItemDataUI(AbilityItemType key, AbilityItemData itemData)
        {
            AbilityItemType = key;
            AbilityItemData = itemData;
            powerUpItemSprite.sprite = AbilityItemData.powerUpItemSprite;
            powerUpItemName.SetText(AbilityItemData.powerUpItemName);
            powerUpItemPrice.SetText(AbilityItemData.powerUpItemPrice.ToString());
        }

        private void OnClickPowerUpButton()
        {
            OnPowerUpItemButtonClicked?.Invoke(this, new OnPowerUpItemButtonClickedEventArgs()
            {
                AbilityItemType = AbilityItemType,
                AbilityItemData = AbilityItemData,
            });
        }
    }
}