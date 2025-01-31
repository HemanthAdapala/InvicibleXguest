using System;
using UnityEngine;

namespace InvicibleXGuest.Scripts.Managers
{
    [CreateAssetMenu(fileName = "SupportItemData", menuName = "SupportItemData", order = 1)]
    public class SupportItemData : ScriptableObject
    {
        public ItemData itemData;
    }
    
    [Serializable]
    public class ItemData
    {
        public string itemName;
        public int cost;
        public Sprite icon;
        public GameObject prefab; // The item prefab (e.g., bomb, gun, ability).
        public ItemType itemType; // Enum: Bomb, Gun, Ability, etc.
    }


    public enum ItemType
    {
        Bomb,
        Gun,
        Ability
    }
}