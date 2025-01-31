using System;
using AYellowpaper.SerializedCollections;
using InvicibleXGuest.Scripts.Interfaces;
using InvicibleXGuest.Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "AbilityItemData", menuName = "ScriptableObjects/CompleteGameAbilitiesDataSo", order = 1)]
public class GameSupportItemsDataSo : ScriptableObject
{
    public SerializedDictionary<ItemType,ItemData> gameSupportItemsData;
}

