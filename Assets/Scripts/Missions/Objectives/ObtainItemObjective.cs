using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;


[Serializable]
[SerializableSelectorName("Obtain an Item")]
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