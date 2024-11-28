using System;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public event EventHandler<OnKeyObjectUpdatedEventArgs> OnKeyObjectUpdated;
    public class OnKeyObjectUpdatedEventArgs : EventArgs
    {
        public int keyObjStatusId;
    }
    
    public void UpdatedKeyObjectStatus(int statusId)
    {
        OnKeyObjectUpdated?.Invoke(this, new OnKeyObjectUpdatedEventArgs
        {
            keyObjStatusId = statusId
        });
    }

    public void UpdatedUIAboutKeyObject(int statusId)
    {
        OnKeyObjectUpdated?.Invoke(this, new OnKeyObjectUpdatedEventArgs
        {
            keyObjStatusId = statusId
        });
    }
}
