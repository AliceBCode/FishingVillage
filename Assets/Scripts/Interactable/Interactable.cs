using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Button;
using DNExtensions.Utilities.SerializableSelector;
using FishingVillage.GameActions;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Interactable
{
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

    
    private Outline _outline;
    
    public string InteractableID => interactableID;



    private void Awake()
    {
        if (TryGetComponent(out _outline))
        {
            _outline.enabled = false;
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            _outline.OutlineColor = Color.dodgerBlue;
            _outline.OutlineWidth = 3f;
        }
    }

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
        if (_outline) _outline.enabled = true;
    }

    public void HideInteract()
    {
        InteractPrompt.Instance?.Hide(true);
        if (_outline) _outline.enabled = false;
    }

    public virtual bool CanInteract()
    {
        return canInteract && (!hasInteracted || !limitInteractionsToOnce) && actionsOnInteract.Length > 0;
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
}