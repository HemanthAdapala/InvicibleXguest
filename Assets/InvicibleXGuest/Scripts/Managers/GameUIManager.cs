using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GameUIManager : MonoBehaviour
{
    [FormerlySerializedAs("supportItemsShopPanel")] [SerializeField] private SupportItemsShopUI supportItemsShopUI;
    [SerializeField] private GameUI gameUI;

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if(supportItemsShopUI.gameObject.activeSelf)
            {
                supportItemsShopUI.gameObject.SetActive(false);
            }
            else
            {
                supportItemsShopUI.gameObject.SetActive(true);
            }
        }
    }
}
