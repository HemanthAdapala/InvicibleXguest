using System;
using InvicibleXGuest.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts
{
    public class SupportItemDataHandler : MonoBehaviour
    {
        [SerializeField] private Image itemSprite = null;
        [SerializeField] private TextMeshProUGUI itemName = null;
        [SerializeField] private TextMeshProUGUI itemPrice = null;
        [SerializeField] private Button itemButton = null;
        [SerializeField] private Color[] color;
        [SerializeField] private Image itemBackground;


        public ItemType ItemType { get;private set; }
        public SupportItemData SupportItemData{get;private set;}
        
        public event EventHandler<OnSupportItemButtonClickedEventArgs> OnSupportItemButtonClicked;
        public class  OnSupportItemButtonClickedEventArgs : EventArgs
        {
           public ItemType ItemType;
           public SupportItemData ItemData;
        }


        public void SetData(ItemType key, SupportItemData itemData)
        {
            Button button = itemButton.GetComponent<Button>();
            if (PlayerManager.LocalInstance.IsPlayerHasEnoughCoins(SupportItemData.itemData.cost))
            {
                itemBackground.color = color[1];
                button.interactable = true;
            }
            else
            {
                itemBackground.color = color[0];
                button.interactable = false;
            }
            
            SetItemDataUI(key, itemData);
            itemButton.onClick.AddListener(OnClickPowerUpButton);
        }

        public void SetItemDataUI(ItemType key, SupportItemData itemData)
        {
            ItemType = key;
            SupportItemData = itemData;
            itemSprite.sprite = SupportItemData.itemData.icon;
            itemName.SetText(SupportItemData.itemData.itemName);
            itemPrice.SetText(SupportItemData.itemData.cost.ToString());
        }

        private void OnClickPowerUpButton()
        {
            OnSupportItemButtonClicked?.Invoke(this, new OnSupportItemButtonClickedEventArgs()
            {
                ItemType = ItemType,
                ItemData = SupportItemData,
            });
        }
    }
}