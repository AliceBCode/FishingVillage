using System;
using DNExtensions.Utilities.SerializableSelector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace FishingVillage.Missions.Objectives
{
    [Serializable]
    [MovedFrom("")]
    [SerializableSelectorName("Use Item In Trigger Area", "Item")]
    public class UseItemInTriggerObjective : MissionObjective
{
    [SerializeField] private SOItem item;
    [SerializeField] private string triggerID;
    [SerializeField] private string areaDescription = "Area";

    protected override string Description
    {
        get
        {
            if (!item)
            {
                return $"Use: (No Item Selected) In: {areaDescription}";
            }

            if (!item.Usable)
            {
                return $"Use: {item.Name} (Item Is Not Usable) In: {areaDescription}";
            }
        
            return $"Use: {item.Name} In: {areaDescription}";
        }
    }

    private bool _inTriggerArea;
    
    public override void Initialize()
    {
        _inTriggerArea = false;
        GameEvents.OnTriggerEntered += OnTriggerEntered;
        GameEvents.OnTriggerExited += OnTriggerExited;
        GameEvents.OnItemUsed += OnItemUsed;
    }
    
    public override void Cleanup()
    {
        GameEvents.OnTriggerEntered -= OnTriggerEntered;
        GameEvents.OnTriggerExited -= OnTriggerExited;
        GameEvents.OnItemUsed -= OnItemUsed;
    }



    public override bool Evaluate()
    {
        return false;
    }
    
    private void OnTriggerExited(string triggeredID)
    {
        if (triggeredID == triggerID)
        {
            _inTriggerArea = false;
        }
    }
    
    private void OnTriggerEntered(string triggeredID)
    {
        if (triggeredID == triggerID)
        {
            _inTriggerArea = true;
        }
    }
    
    private void OnItemUsed(SOItem item)
    {
        if (item == this.item && _inTriggerArea)
        {
            SetMet();
        }
    }

    }
}
