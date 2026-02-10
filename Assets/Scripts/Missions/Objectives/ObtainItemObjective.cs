using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Obtain Item", "Item")]
    public class ObtainItemObjective : MissionObjective
{
    [SerializeField] private SOItem requiredItem;
    
    protected override string Description => $"Obtain {(requiredItem ? requiredItem.Name : "Unknown Item")}";
    
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
        return Player.PlayerInventory.Instance && Player.PlayerInventory.Instance.HasItem(requiredItem);
    }

    private void OnItemObtained(SOItem item)
    {
        if (item == requiredItem)
        {
            SetMet();
        }
    }
    }
}