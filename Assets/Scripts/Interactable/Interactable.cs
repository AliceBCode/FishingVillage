using System;
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
    [SerializeField, ReadOnly] private string interactableID = "";

    public string InteractableID => interactableID;

    
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

    [Button]
    public void Interact()
    {
        if (limitInteractionsToOnce && hasInteracted)  return;
        hasInteracted = true;
        onInteract?.Invoke();
        GameEvents.InteractedWith(this);
        OnInteract();
    }
    
        
    public virtual bool CanInteract()
    {
        return canInteract;
    }
    
    protected abstract void OnInteract();

}