using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyObjectUpdaterData", menuName = "ScriptableObjects/KeyObjectUpdaterDataSO", order = 1)]
public class KeyObjectUpdaterDataSO : ScriptableObject
{
    public SerializedDictionary<int,string> keyObjectUpdaterData = new SerializedDictionary<int, string>();
}
