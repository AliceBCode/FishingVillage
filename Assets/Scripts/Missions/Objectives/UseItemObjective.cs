using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Use Item", "Item")]
    public class UseItemObjective : MissionObjective
{
    [SerializeField] private SOItem item;

    protected override string Description
    {
        get
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
    }
}