using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool limitInteractionsToOnce;
    [SerializeReference, SubclassSelector] 
    private GameAction[] actionsOnInteract = Array.Empty<GameAction>();
    
    [SerializeField, ReadOnly] private bool hasInteracted;
    [SerializeField, ReadOnly] private string interactableID = "";

    public string InteractableID => interactableID;



    [Button]
    public void Interact()
    {
        if (limitInteractionsToOnce && hasInteracted) return;
        
        hasInteracted = true;
        
        GameEvents.InteractedWith(this);
        OnInteract();
        
        foreach (var action in actionsOnInteract)
        {
            action?.Execute();
        }
    }
    
    public virtual bool CanInteract()
    {
        return canInteract;
    }
    
    protected abstract void OnInteract();
    
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(interactableID))
        {
            interactableID = Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    
    [Button(ButtonPlayMode.OnlyWhenNotPlaying)]
    private void RegenerateID()
    {
        interactableID = Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}