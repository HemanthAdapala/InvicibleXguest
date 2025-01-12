using System;
using UnityEngine;

namespace InvicibleXGuest.Scripts.Interfaces
{
    public interface IAbilityHandler
    {
        void Initialize(AbilityItemData abilityItemData);

        void InitializeAbilityHandler();// Initialize the ability
        void Cleanup(); // Optional cleanup logic
    }
}