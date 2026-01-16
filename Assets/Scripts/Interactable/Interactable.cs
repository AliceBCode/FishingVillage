using DNExtensions;
using DNExtensions.Button;
using UnityEngine;
using UnityEngine.Events;


[SelectionBase]
[DisallowMultipleComponent]
public abstract class Interactable : MonoBehaviour, IInteractable
{
    
    [Header("Interactable Settings")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool limitInteractionsToOnce;
    [Space(10)]
    [SerializeField] private UnityEvent onInteract;
    
    [SerializeField, ReadOnly] private bool hasInteracted;

    


    [Button]
    public void Interact()
    {
        if (limitInteractionsToOnce && hasInteracted)  return;
        hasInteracted = true;
        onInteract?.Invoke();
        OnInteract();
    }
    
        
    public virtual bool CanInteract()
    {
        return canInteract;
    }
    
    protected abstract void OnInteract();

}