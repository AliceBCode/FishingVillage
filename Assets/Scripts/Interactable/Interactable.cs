using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.UI;
using TMPro;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] protected bool canInteract = true;
    [SerializeField] protected bool limitInteractionsToOnce;
    [SerializeField] protected Vector3 promptOffset = Vector3.up;
    [SerializeReference, SerializableSelector] 
    protected GameAction[] actionsOnInteract = Array.Empty<GameAction>();
    [SerializeField, ReadOnly] protected bool hasInteracted;
    [SerializeField, ReadOnly] protected string interactableID = "";

    public string InteractableID => interactableID;



    [Button]
    public void Interact()
    {
        if (limitInteractionsToOnce && hasInteracted) return;
        
        hasInteracted = true;
        
        foreach (var action in actionsOnInteract)
        {
            action?.Execute();
        }
        
        GameEvents.InteractedWith(this);
        OnInteract();
    }

    public virtual void ShowInteract()
    {
        if (!CanInteract()) return;
        
        InteractPrompt.Instance?.Show(transform.position + promptOffset);
    }

    public void HideInteract()
    {
        InteractPrompt.Instance?.Hide(true);
    }

    public virtual bool CanInteract()
    {
        return canInteract && (!hasInteracted || !limitInteractionsToOnce);
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