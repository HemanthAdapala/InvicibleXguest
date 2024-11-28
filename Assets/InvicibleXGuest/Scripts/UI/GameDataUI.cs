using System;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;

public class GameDataUI : MonoBehaviour
{
    [SerializeField] private KeyObjectUpdaterDataSO keyObjectUpdaterDataSO;
    [SerializeField] private TextMeshProUGUI keyObjectUpdater;
    
    private SerializedDictionary<int,string> keyObjectUpdaterData;
    

    private void Awake()
    {
        GameUIController.Instance.OnKeyObjectUpdated += OnKeyObjectUpdated;
        keyObjectUpdaterData = new SerializedDictionary<int, string>();
        keyObjectUpdaterData = keyObjectUpdaterDataSO.keyObjectUpdaterData;
        keyObjectUpdater.SetText(keyObjectUpdaterData[1]);
        
    }

    private void OnKeyObjectUpdated(object sender, GameUIController.OnKeyObjectUpdatedEventArgs e)
    {
        keyObjectUpdater.SetText(keyObjectUpdaterData[e.keyObjStatusId]);
    }
}
