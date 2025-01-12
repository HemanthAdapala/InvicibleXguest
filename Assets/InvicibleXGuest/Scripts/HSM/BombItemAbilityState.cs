using InvicibleXGuest.Scripts;
using InvicibleXGuest.Scripts.Interfaces;
using InvicibleXGuest.Scripts.Managers;
using UnityEngine;

public class BombItemAbilityState : ItemAbilityBaseState
{
    private IAbilityHandler surfaceHandler;
    
    public override void EnterState(AbilityStateMachineManager stateMachine)
    {
        surfaceHandler = PlayerManager.LocalInstance.GetAbility<IAbilityHandler>();
    }

    public override void ExitState(AbilityStateMachineManager stateMachine)
    {
        Debug.Log("Exiting BombItemAbilityState");
        surfaceHandler.Cleanup();
    }

    public override void Update(AbilityStateMachineManager stateMachine)
    {
        if (surfaceHandler != null)
        {
            surfaceHandler.InitializeAbilityHandler();
        }
        else
        {
            Debug.LogWarning("StickyBombAbilityBaseHandler not equipped.");
        }
    }
}
