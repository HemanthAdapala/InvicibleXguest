using UnityEngine;

public class SupportItemStateMachine
{
    public SupportItemStateMachine(PlayerController playerController)
    {
        bombSupportItemState = new BombSupportItemState(playerController);
        visibilitySupportItemState = new VisibilitySupportItemState(playerController);
    }

    private ISupportItemBaseState _currentState;

    public BombSupportItemState bombSupportItemState;
    
    public VisibilitySupportItemState visibilitySupportItemState;
    
    
    public void Initialize(ISupportItemBaseState supportItemBaseState)
    {
        _currentState = supportItemBaseState;
        _currentState.EnterState();
    }
    
    public void TransitionToState(ISupportItemBaseState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    public void Update()
    {
        _currentState?.Update();
    }
    
}
