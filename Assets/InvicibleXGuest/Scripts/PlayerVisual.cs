using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour 
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    private Material[] _skinnedMeshRendererMaterial;

    private void Awake() {
        _skinnedMeshRendererMaterial = skinnedMeshRenderer.materials;
    }

    public void SetPlayerColor(Color color) 
    {
        foreach (var material in _skinnedMeshRendererMaterial)
        {
            material.color = color;
        }
    }

}