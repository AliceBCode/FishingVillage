namespace FishingVillage.Interactable
{
    public interface IInteractable
    {
        bool CanInteract();
        void Interact();
        void ShowInteract();
        void HideInteract();
    }
}
