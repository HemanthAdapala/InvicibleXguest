using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class KeyObject : NetworkBehaviour
{
    public bool IsHeld { get; private set; }
    
    public void Initialize(bool isHeld = false)
    {
        Debug.Log("KeyObject Initialized" + isHeld);
        IsHeld = isHeld;
    }
    
    [ClientRpc]
    public void RotateKeyObjectClientRpc()
    {
        StartCoroutine(RotateKeyObject());
    }

    public void SetHeldState(bool isHeld)
    {
        IsHeld = isHeld;
        if (isHeld)
        {
            StopCoroutine(RotateKeyObject());               
        }
        else
        {
            StartCoroutine(RotateKeyObject());
        }
    }

    IEnumerator RotateKeyObject()
    {
        while (!IsHeld)
        {
            gameObject.transform.Rotate(new Vector3(0,1),90 * Time.deltaTime);       
            yield return null;
        }
    }
}