using UnityEngine;

public class AbilityStateMachineManager : MonoBehaviour
{
    private ItemAbilityBaseState currentState;
    public BombItemAbilityState bombItemAbilityState;
    public VisibilityItemAbilityState visibilityItemAbilityState;


    private void Start()
    {
        bombItemAbilityState = new BombItemAbilityState();
        visibilityItemAbilityState = new VisibilityItemAbilityState();
    }

    public void UpdateStateMachine()
    {
        currentState?.Update(this);
    }
    
    public void TransitionToState(ItemAbilityBaseState state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void Initialize()
    {
        currentState = null;
        Debug.Log("AbilityStateMachineManager Initialized");
    }
}
