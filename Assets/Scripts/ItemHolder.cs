


using DNExtensions;
using UnityEngine;

[SelectionBase]
public class ItemHolder : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private SOItem item;
    [Tooltip("Amount of item to give when opened, 0 is infinite.")]
    [SerializeField, Min(0)] private int amount = 1;
    [SerializeField, ReadOnly] private int itemsWhereTaken;
    
    public void TakeItem()
    {
        if (amount != 0 && itemsWhereTaken >= amount) return;

        if (PlayerController.Instance.Inventory.TryAddItem(item))
        {
            itemsWhereTaken++;
        }

    }
    
}
