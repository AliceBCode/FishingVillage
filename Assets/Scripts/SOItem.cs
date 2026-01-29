using System;
using DNExtensions.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class SOItem : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField, Preview] private Sprite icon;
    [SerializeField] private bool usable;
    [SerializeField, ShowIf("usable")] private Usage usage;
    
    private enum Usage
    {
        CleaningUtensils = 0,
        Horn = 1,
    }
    
    
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
    public bool Usable => usable;


    public void Use()
    {
        if (!usable) return;
        
        switch (usage)
        {
            case Usage.CleaningUtensils:
                GameEvents.UsedCleaningUtensils();
                break;
            case Usage.Horn:
                GameEvents.UsedHorn();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        GameEvents.ItemUsed(this);
    }
}