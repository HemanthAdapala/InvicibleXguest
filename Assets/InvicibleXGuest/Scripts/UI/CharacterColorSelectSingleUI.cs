using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour {


    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            iXgGameMultiplayer.Instance.ChangePlayerColor(colorId);
        });
    }
    
    private void Start() {
        iXgGameMultiplayer.Instance.OnNetworkPlayerListChangedEvent += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        image.color = iXgGameMultiplayer.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }
    
    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdateIsSelected();
    }
    
    private void UpdateIsSelected() { 
        if (iXgGameMultiplayer.Instance.GetPlayerData().colorId == colorId) {
            selectedGameObject.SetActive(true);
        } else {
            selectedGameObject.SetActive(false);
        }
    }
    
    private void OnDestroy() {
        iXgGameMultiplayer.Instance.OnNetworkPlayerListChangedEvent -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }
}