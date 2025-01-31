using System;
using System.Collections.Generic;
using UnityEngine;

namespace InvicibleXGuest.Scripts.Managers
{
    /// <summary>
    /// Manages abilities by adding, removing, and retrieving them.
    /// </summary>
    public class SupportItemEquipmentManager : MonoBehaviour
    {
        private SupportItemStateMachine _supportItemStateMachine;
        
        private List<SupportItemData> _ownedSupportItems;
        
        private SupportItemData _currentSupportItemData;

        private void Awake()
        {
            _ownedSupportItems = new List<SupportItemData>();
            _supportItemStateMachine = new SupportItemStateMachine(GetComponentInParent<PlayerController>());
        }
        
        public void AddSupportItem(SupportItemData item)
        {
            _ownedSupportItems.Add(item);
        }
        
        public void RemoveSupportItem(SupportItemData item)
        {
            _ownedSupportItems.Remove(item);
        }
        
        public void EquipSupportItem(SupportItemData item)
        {
            if (!_ownedSupportItems.Contains(item)) return; // Ensure the player owns the item.

            _currentSupportItemData = item;
            // Transition the state machine to the correct state based on the item type.
            switch (item.itemData.itemType)
            {
                case ItemType.Bomb:
                    _supportItemStateMachine.TransitionToState(_supportItemStateMachine.bombSupportItemState);
                    break;
                case ItemType.Ability:
                    _supportItemStateMachine.TransitionToState(_supportItemStateMachine.visibilitySupportItemState);
                    break;
            }
        }

        private void Update()
        {
            _supportItemStateMachine.Update();
        }
    }   
}