using System;
using InvicibleXGuest.Scripts;
using InvicibleXGuest.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AbilityItemsShopPanelHandler : MonoBehaviour , IBaseGameUI
{
    [SerializeField] private Transform powerUpItemPrefab;
    [SerializeField] private Transform powerUpStoreItemParentTransform;
    [SerializeField] private Transform powerUpEquipItemParentTransform;
    [SerializeField] private TextMeshProUGUI playerTotalCoinsText;
    [FormerlySerializedAs("gamePowerUpItemDataSo")] [SerializeField] private GameAbilitiesDataSo gameAbilitiesDataSo;
    
    //Add these items to powerUp panel UI
    public event EventHandler<OnPowerUpItemBoughtEventArgs> OnPowerUpItemBought;
    public class OnPowerUpItemBoughtEventArgs : EventArgs
    {
        public AbilityItemType AbilityItemType;
        public AbilityItemData AbilityItemData;
    }

    private void Awake()
    {
        Hide();
    }

    private void Start()
    {
        InitializeAbilityShopItems();
    }

    private void OnEnable()
    {
        InitializeAbilityShopItems();
    }
    
    //Testing purpose for show the PowerUp store
    private void OnDisable()
    {
        foreach (Transform child in powerUpStoreItemParentTransform)
        {
            //Disable the children game objects
            child.gameObject.SetActive(false);
        }
    }
    
    private void InitializeAbilityShopItems()
    {
        //TODO:- Add the player coins data after validating from server using RPC
        playerTotalCoinsText.SetText(PlayerManager.LocalInstance.PlayerCoins.ToString());
        if (powerUpStoreItemParentTransform.childCount == 0)
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
        foreach (var powerUpItem in gameAbilitiesDataSo.powerUpItems)
        {
            var powerUpItemTransform = Instantiate(powerUpItemPrefab, powerUpStoreItemParentTransform);
            var powerUpItemDataHandler = powerUpItemTransform.GetComponent<AbilityItemDataHandler>();
            powerUpItemDataHandler.SetData(powerUpItem.Key,powerUpItem.Value);
            powerUpItemDataHandler.OnPowerUpItemButtonClicked += OnPowerUpItemButtonClicked;
        }   
    }
    
    private void ValidateOnEnable()
    {
        foreach (Transform child in powerUpStoreItemParentTransform)
        {
            //Enable the children game objects
            child.gameObject.SetActive(true);
        }
    }

    private void ValidateAlreadyBoughtItems()
    {
        var equipItems = PlayerManager.LocalInstance.GetEquipItems();
        if (equipItems.Count == 0) return;

        foreach (var boughtItem in equipItems)
        {
            bool itemFound = false;
            foreach (Transform child in powerUpEquipItemParentTransform)
            {
                if (child.GetComponent<AbilityItemDataHandler>().AbilityItemType == boughtItem.Key)
                {
                    child.gameObject.SetActive(true);
                    itemFound = true;
                    break;
                }
            }
            if (!itemFound)
            {
                var powerUpItemTransform = Instantiate(powerUpItemPrefab, powerUpEquipItemParentTransform);
                powerUpItemTransform.GetComponent<AbilityItemDataHandler>().SetItemDataUI(boughtItem.Key, boughtItem.Value);
            }
        }
    }

    
    private void OnPowerUpItemButtonClicked(object sender, AbilityItemDataHandler.OnPowerUpItemButtonClickedEventArgs e)
    {
        var itemPrice = e.AbilityItemData.powerUpItemPrice;
        if (PlayerManager.LocalInstance.IsPlayerHasEnoughCoins(itemPrice))
        {
            PlayerManager.LocalInstance.UpdateCoinsData(itemPrice);
            
            playerTotalCoinsText.SetText(PlayerManager.LocalInstance.PlayerCoins.ToString());
            Debug.Log("Player has enough coins to purchase the power up");
            AddToEquipItems(e.AbilityItemType,e.AbilityItemData);
        }
        else
        {
            Debug.Log("Player does not have enough coins to purchase the power up");
        }
        Debug.Log("PowerUp item type" + e.AbilityItemType + " Price:- " + itemPrice);
    }
    
    //Add equip items to the player after purchased from the store
    private void AddToEquipItems(AbilityItemType itemType,AbilityItemData abilityItemData)
    {
        var powerUpItemTransform = Instantiate(powerUpItemPrefab, powerUpEquipItemParentTransform);
        PlayerManager.LocalInstance.AddToEquipItems(itemType,abilityItemData);
        powerUpItemTransform.GetComponent<AbilityItemDataHandler>().SetItemDataUI(itemType,abilityItemData);
        OnPowerUpItemBought?.Invoke(this, new OnPowerUpItemBoughtEventArgs()
        {
            AbilityItemType = itemType,
            AbilityItemData = abilityItemData,
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
