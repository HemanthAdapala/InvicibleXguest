using UnityEngine;

public abstract class ItemAbilityBaseState
{
    public abstract void EnterState(AbilityStateMachineManager stateMachine);
    public abstract void ExitState(AbilityStateMachineManager stateMachine);
    public abstract void Update(AbilityStateMachineManager stateMachine);
}
