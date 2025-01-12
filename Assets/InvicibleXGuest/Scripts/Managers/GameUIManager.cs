using System;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private AbilityItemsShopPanelHandler abilityItemsShopPanelHandler;
    [SerializeField] private GameUI gameUI;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(abilityItemsShopPanelHandler.gameObject.activeSelf)
            {
                abilityItemsShopPanelHandler.gameObject.SetActive(false);
            }
            else
            {
                abilityItemsShopPanelHandler.gameObject.SetActive(true);
            }
        }
    }
}
