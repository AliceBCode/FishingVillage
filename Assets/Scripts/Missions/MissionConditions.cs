using System;
using UnityEngine;

[Serializable]
public abstract class MissionCondition
{
    public static event Action OnConditionMet;
    
    public bool Met { get; protected set; }


    
    public abstract string Name { get; }
    public abstract string Description { get; }
    
    public abstract void Initialize();
    public abstract void Cleanup();
    public abstract bool Evaluate();
    
    protected void SetMet()
    {
        if (Met) return;
        Met = true;
        OnConditionMet?.Invoke();
    }
}


[Serializable]
[AddTypeMenu("Obtain an Item")]
public class ObtainItemCondition : MissionCondition
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
        return PlayerController.Instance && PlayerController.Instance.Inventory.HasItem(requiredItem);
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
public class GiveItemToNPCCondition : MissionCondition
{
    [SerializeField] private SOItem requiredItem;
    [SerializeField] private string npcName;
    
    public override string Name => "Give Item To NPC";
    public override string Description => $"Give {(requiredItem ? requiredItem.Name : "Unknown Item")} to {npcName}";
    
    public override void Initialize()
    {
        GameEvents.OnItemGivenToNPC += OnItemGivenToNPC;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnItemGivenToNPC -= OnItemGivenToNPC;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnItemGivenToNPC(SOItem item, NPC npc)
    {
        if (item == requiredItem && npc != null && npc.Name == npcName)
        {
            SetMet();
        }
    }
}




[Serializable]
[AddTypeMenu("Interact with an NPC")]
public class TalkToNPCCondition : MissionCondition
{
    [SerializeField] private string npcName;
    
    public override string Name => "Talk To NPC";
    public override string Description => $"Talk to {npcName}";
    
    public override void Initialize()
    {
        GameEvents.OnNPCTalkedTo += OnNPCTalkedTo;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnNPCTalkedTo -= OnNPCTalkedTo;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnNPCTalkedTo(NPC npc)
    {
        if (npc && npc.Name == npcName)
        {
            SetMet();
        }
    }
}


[Serializable]
[AddTypeMenu("Enter a trigger area")]
public class EnterTriggerCondition : MissionCondition
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