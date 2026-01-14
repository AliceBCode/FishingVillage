using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SOItem : ScriptableObject
{
    [SerializeField] private new string name;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
}