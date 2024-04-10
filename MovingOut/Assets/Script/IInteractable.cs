namespace Script
{
    public interface IInteractable
    {
        public void Interact(Agent agent);
        
        public void DeInteract(Agent agent);
    }
}