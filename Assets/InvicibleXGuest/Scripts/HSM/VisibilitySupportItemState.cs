public class VisibilitySupportItemState : ISupportItemBaseState
{
    public PlayerController PlayerController { get; set; }
    
    public VisibilitySupportItemState(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }
    
    public void EnterState()
    {
        
    }

    public void ExitState()
    {
        
    }

    public void Update()
    {
        
    }
}
