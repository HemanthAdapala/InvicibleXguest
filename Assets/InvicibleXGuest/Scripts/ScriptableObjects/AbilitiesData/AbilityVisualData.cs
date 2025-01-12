using UnityEngine;

[CreateAssetMenu(fileName = "AbilityVisualData", menuName = "ScriptableObjects/AbilityVisualData", order = 1)]
public class AbilityVisualData : ScriptableObject
{
    public GameObject abilityPrefab; // Prefab for the ability
    public GameObject visualEffectPrefab;
}
