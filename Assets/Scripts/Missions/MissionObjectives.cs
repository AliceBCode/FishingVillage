using System;
using UnityEngine;

[Serializable]
public abstract class MissionObjective
{
    public static event Action OnObjectiveMet;
    
    [SerializeField] private bool isHidden;
    [SerializeField] private bool requiresPreviousObjective;
    
    public bool IsHidden => isHidden;
    public bool RequiresPreviousObjective => requiresPreviousObjective;
    
    public bool Met { get; protected set; }
    public bool IsActive { get; private set; } = true;


    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract void Initialize();
    public abstract void Cleanup();
    public abstract bool Evaluate();
    
    protected void SetMet()
    {
        if (Met || !IsActive) return;
        
        Met = true;
        OnObjectiveMet?.Invoke();
    }
    
    public void SetActive(bool active)
    {
        IsActive = active;
    }
}


[Serializable]
[AddTypeMenu("Obtain an Item")]
public class ObtainItemObjective : MissionObjective
{
    [SerializeField] private SOItem requiredItem;
    
    public override string Name => "Obtain Item";
    public override string Description => $"Obtain {(requiredItem ? requiredItem.Name : "Unknown Item")}";
    
    public override void Initialize()
    {
        GameEvents.OnItemObtained += OnItemObtained;
        
        if (Evaluate())
        {
            SetMet();
        }
    }
    
    public override void Cleanup()
    {
        GameEvents.OnItemObtained -= OnItemObtained;
    }
    
    public override bool Evaluate()
    {
        if (!requiredItem) return false;
        return PlayerInventory.Instance && PlayerInventory.Instance.HasItem(requiredItem);
    }
    
    private void OnItemObtained(SOItem item)
    {
        if (item == requiredItem)
        {
            SetMet();
        }
    }
}

[Serializable]
[AddTypeMenu("Give Item to an NPC")]
public class GiveItemToNpcObjective : MissionObjective
{
    [SerializeField] private SOItem requiredItem;
    [SerializeField] private NPC npcReference;
    
    public SOItem RequiredItem => requiredItem;
    public override string Name => "Give Item To NPC";
    public override string Description => $"Give {(requiredItem ? requiredItem.Name : "Unknown Item")} to {npcReference}";
    
    private string targetID;
    
    
    public override void Initialize()
    {
        if (!npcReference)
        {
            Debug.LogError("No NPC prefab reference set in objective!");
            return;
        }
        
        targetID = npcReference.InteractableID;
        
        if (string.IsNullOrEmpty(targetID))
        {
            Debug.LogError($"NPC prefab {npcReference.name} has no ID set!");
            return;
        }

        
        GameEvents.OnItemGivenToNpc += OnItemGivenToNPC;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnItemGivenToNpc -= OnItemGivenToNPC;
    }
    
    public override bool Evaluate()
    {
        return false;
    }

    public bool IsNpc(NPC npc)
    {
        return npc && npc.InteractableID == targetID;
    }
    
    private void OnItemGivenToNPC(SOItem item, NPC npc)
    {
        if (item == requiredItem && IsNpc(npc))
        {
            SetMet();
        }
    }
}




[Serializable]
[AddTypeMenu("Talk to NPC")]
public class TalkToNpcObjective : MissionObjective
{
    [SerializeField] private NPC npcReference;
    
    private string targetID;
    
    public override string Name => "Talk To NPC";
    public override string Description => $"Talk to {(npcReference ? npcReference.name : "Unknown")}";
    
    public override void Initialize()
    {
        if (!npcReference)
        {
            Debug.LogError("No NPC prefab reference set in objective!");
            return;
        }
        
        targetID = npcReference.InteractableID;
        
        if (string.IsNullOrEmpty(targetID))
        {
            Debug.LogError($"NPC prefab {npcReference.name} has no ID set!");
            return;
        }
        
        GameEvents.OnNpcTalkedTo += OnNPCTalkedTo;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnNpcTalkedTo -= OnNPCTalkedTo;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnNPCTalkedTo(NPC npc)
    {
        if (npc && npc.InteractableID == targetID)
        {
            SetMet();
        }
    }
}

[Serializable]
[AddTypeMenu("Interact with")]
public class InteractWithObjective : MissionObjective
{
    [SerializeField] private Interactable interactableReference;
    
    private string targetID;
    
    public override string Name => "Interact with";
    public override string Description => $"Interact with {(interactableReference ? interactableReference.name : "Unknown")}";
    
    public override void Initialize()
    {
        if (!interactableReference)
        {
            Debug.LogError("No Interactable prefab reference set in objective!");
            return;
        }
        
        targetID = interactableReference.InteractableID;
        
        if (string.IsNullOrEmpty(targetID))
        {
            Debug.LogError($"Interactable prefab {interactableReference.name} has no ID set!");
            return;
        }
        
        GameEvents.OnInteractedWith += OnInteractedWith;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnInteractedWith -= OnInteractedWith;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnInteractedWith(Interactable interactable)
    {
        if (interactable && interactable.InteractableID == targetID)
        {
            SetMet();
        }
    }
}


[Serializable]
[AddTypeMenu("Enter a trigger area")]
public class EnterTriggerObjective : MissionObjective
{
    [SerializeField] private string triggerID;
    
    public override string Name => "Enter Trigger";
    public override string Description => $"Enter trigger: {triggerID}";
    
    public override void Initialize()
    {
        GameEvents.OnTriggerEntered += OnTriggerEntered;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnTriggerEntered -= OnTriggerEntered;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnTriggerEntered(string triggeredID)
    {
        if (triggeredID == triggerID)
        {
            SetMet();
        }
    }
}


[Serializable]
[AddTypeMenu("Jump action")]
public class JumpActionObjective : MissionObjective
{
    public override string Name => "Jump Action";
    public override string Description => $"Jump";
    
    public override void Initialize()
    {
        GameEvents.OnJumpedAction += OnJumpAction;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnJumpedAction -= OnJumpAction;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnJumpAction()
    {
        SetMet();
    }
}

[Serializable]
[AddTypeMenu("Walk action")]
public class WalkActionObjective : MissionObjective
{
    public override string Name => "Walk Action";
    public override string Description => $"Walk";
    
    public override void Initialize()
    {
        GameEvents.OnWalkAction += OnWalkAction;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnWalkAction -= OnWalkAction;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnWalkAction()
    {
        SetMet();
    }
}

[Serializable]
[AddTypeMenu("Interact sequence")]
public class InteractSequenceObjective : MissionObjective
{
    [SerializeField] private Interactable[] requiredSequence;
    
    private string[] targetIDs;
    private int currentIndex;
    
    public override string Name => "Interact Sequence";
    public override string Description => $"Interact in order ({currentIndex}/{requiredSequence.Length})";
    
    public override void Initialize()
    {
        currentIndex = 0;
    
        targetIDs = new string[requiredSequence.Length];
        for (int i = 0; i < requiredSequence.Length; i++)
        {
            if (requiredSequence[i])
            {
                targetIDs[i] = requiredSequence[i].InteractableID;
            }
            else
            {
                Debug.LogError($"Sequence objective has null reference at index {i}!");
            }
        }
    
        GameEvents.OnInteractedWith += OnInteraction;
    }

    public override void Cleanup()
    {
        GameEvents.OnInteractedWith -= OnInteraction;
    }
    
    public override bool Evaluate()
    {
        return currentIndex >= requiredSequence.Length;
    }
    
    private void OnInteraction(Interactable interactable)
    {
        if (currentIndex >= targetIDs.Length) return;
        
        bool isPartOfSequence = Array.Exists(targetIDs, id => id == interactable.InteractableID);
        if (!isPartOfSequence) return; 
        
        
        if (interactable.InteractableID == targetIDs[currentIndex])
        {
            currentIndex++;
            
            if (Evaluate())
            {
                SetMet();
            }
        }
        else 
        {
            currentIndex = 0;
        }
    }
}