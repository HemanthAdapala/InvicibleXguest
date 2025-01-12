using System;
using AYellowpaper.SerializedCollections;
using InvicibleXGuest.Scripts.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AbilityItemData", menuName = "ScriptableObjects/CompleteGameAbilitiesDataSo", order = 1)]
public class GameAbilitiesDataSo : ScriptableObject
{
    public SerializedDictionary<AbilityItemType,AbilityItemData> powerUpItems;
}

[System.Serializable]
public struct AbilityItemData
{
    [Tooltip("The visual data for the ability.")]
    public AbilityVisualData abilityVisualData;
    [Tooltip("The ability handler for the ability.")]
    public GameObject abilityHandler;
    [Tooltip("The ability handler for the ability.")]
    public Sprite powerUpItemSprite;
    [Tooltip("The ability handler for the ability.")]
    public string powerUpItemName;
    [Tooltip("The ability handler for the ability.")]
    public int powerUpItemPrice;
    [Tooltip("The ability handler for the ability.")]
    public bool isEquipped;
    [Tooltip("The ability handler for the ability.")]
    public bool isPurchased;
}

public enum AbilityItemType
{
    None,
    StickyBomb,
}

