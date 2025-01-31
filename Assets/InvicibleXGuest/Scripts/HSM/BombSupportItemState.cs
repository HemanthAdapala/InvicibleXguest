public class BombSupportItemState : ISupportItemBaseState
{
    public PlayerController PlayerController;
    
    public BombSupportItemState(PlayerController playerController)
    {
        this.PlayerController = playerController;
    }
    
    public void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
