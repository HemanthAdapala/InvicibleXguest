using System;
using InvicibleXGuest.Scripts;
using InvicibleXGuest.Scripts.Interfaces;
using InvicibleXGuest.Scripts.Managers;
using TMPro;
using UnityEngine;

public class SupportItemsShopUI : MonoBehaviour , IBaseGameUI
{
    [SerializeField] private Transform supportItemPrefab;
    [SerializeField] private Transform storeItemParentTransform;
    [SerializeField] private Transform equipItemParentTransform;
    [SerializeField] private TextMeshProUGUI playerTotalCoinsText;
    [SerializeField] private GameSupportItemsDataSo gameSupportItemDataSo;
    
    
    public event EventHandler<OnPowerUpItemBoughtEventArgs> OnPowerUpItemBought;

    public class OnPowerUpItemBoughtEventArgs
    {
        public ItemType ItemType;
        public SupportItemData ItemData;
    }
    
    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        InitializeSupportShopItems();
    }

    private void OnEnable()
    {
        InitializeSupportShopItems();
    }
    
    //Testing purpose for show the PowerUp store
    private void OnDisable()
    {
        foreach (Transform child in storeItemParentTransform)
        {
            //Disable the children game objects
            child.gameObject.SetActive(false);
        }
    }
    
    private void InitializeSupportShopItems()
    {
        //TODO:- Add the player coins data after validating from server using RPC
        playerTotalCoinsText.SetText(PlayerManager.LocalInstance.PlayerCoins.ToString());
        if (storeItemParentTransform.childCount == 0)
        {
            PopulatePowerUpShopItems();
            Debug.Log("PowerUp Shop Items Populated");
        }
        else
        {
            ValidateOnEnable();
            Debug.Log("PowerUp Shop Items Already Populated");
        }   
        ValidateAlreadyBoughtItems();
    }
    
    private void PopulatePowerUpShopItems()
    {
        foreach (var powerUpItem in gameSupportItemDataSo.gameSupportItemsData)
        {
            var powerUpItemTransform = Instantiate(supportItemPrefab, storeItemParentTransform);
            var powerUpItemDataHandler = powerUpItemTransform.GetComponent<SupportItemDataHandler>();
            //powerUpItemDataHandler.SetData(powerUpItem.Key,powerUpItem.Value);
            powerUpItemDataHandler.OnSupportItemButtonClicked += OnSupportItemButtonClicked;
        }   
    }
    
    private void ValidateOnEnable()
    {
        foreach (Transform child in storeItemParentTransform)
        {
            //Enable the children game objects
            child.gameObject.SetActive(true);
        }
    }

    private void ValidateAlreadyBoughtItems()
    {
        // var equipItems = PlayerManager.LocalInstance.GetEquipItems();
        // if (equipItems.Count == 0) return;
        //
        // foreach (var boughtItem in equipItems)
        // {
        //     bool itemFound = false;
        //     foreach (Transform child in equipItemParentTransform)
        //     {
        //         if (child.GetComponent<SupportItemDataHandler>().ItemType == boughtItem.Key)
        //         {
        //             child.gameObject.SetActive(true);
        //             itemFound = true;
        //             break;
        //         }
        //     }
        //     if (!itemFound)
        //     {
        //         var powerUpItemTransform = Instantiate(supportItemPrefab, equipItemParentTransform);
        //         powerUpItemTransform.GetComponent<SupportItemDataHandler>().SetItemDataUI(boughtItem.Key, boughtItem.Value);
        //     }
        // }
    }

    
    private void OnSupportItemButtonClicked(object sender, SupportItemDataHandler.OnSupportItemButtonClickedEventArgs e)
    {
        var itemPrice = e.ItemData.itemData.cost;
        if (PlayerManager.LocalInstance.IsPlayerHasEnoughCoins(itemPrice))
        {
            PlayerManager.LocalInstance.UpdateCoinsData(itemPrice);
            
            playerTotalCoinsText.SetText(PlayerManager.LocalInstance.PlayerCoins.ToString());
            Debug.Log("Player has enough coins to purchase the power up");
            AddToEquipItems(e.ItemType,e.ItemData);
        }
        else
        {
            Debug.Log("Player does not have enough coins to purchase the power up");
        }
        Debug.Log("PowerUp item type" + e.ItemType + " Price:- " + itemPrice);
    }
    
    //Add equip items to the player after purchased from the store
    private void AddToEquipItems(ItemType itemType,SupportItemData itemData)
    {
        var powerUpItemTransform = Instantiate(supportItemPrefab, equipItemParentTransform);
        //PlayerManager.LocalInstance.AddToEquipItems(itemType,itemData);
        powerUpItemTransform.GetComponent<SupportItemDataHandler>().SetItemDataUI(itemType,itemData);
        OnPowerUpItemBought?.Invoke(this, new OnPowerUpItemBoughtEventArgs()
        {
            ItemType = itemType,
            ItemData = itemData,
        });
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}


