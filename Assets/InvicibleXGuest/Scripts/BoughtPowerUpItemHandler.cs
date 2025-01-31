using System;
using InvicibleXGuest.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace InvicibleXGuest.Scripts
{
    public class BoughtPowerUpItemHandler : MonoBehaviour
    {
        [SerializeField] private Image powerUpItemSprite = null;
        
        public void SetUiData(ItemType key, SupportItemData itemData)
        {
            powerUpItemSprite.sprite = itemData.itemData.icon;       
            Button button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}