using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Use Item", "Item")]
public class UseItemObjective : MissionObjective
{
    [SerializeField] private SOItem item;

    public override string Description => GetDescription();
    
    
    public override void Initialize()
    {
        GameEvents.OnItemObtained += OnItemUsed;
        
        if (Evaluate())
        {
            SetMet();
        }
    }
    
    public override void Cleanup()
    {
        GameEvents.OnItemUsed -= OnItemUsed;
    }
    
    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnItemUsed(SOItem item)
    {
        if (item == this.item)
        {
            SetMet();
        }
    }

    private string GetDescription()
    {
        if (!item)
        {
            return $"Use: (No Item Selected)";
        }

        if (!item.Usable)
        {
            return $"Use: {item.Name} (Item Is Not Usable)";
        }
        
        return $"Use: {item.Name}";
    }
}