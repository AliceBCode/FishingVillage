


using DNExtensions;
using UnityEngine;


public class ItemHolder : Interactable
{

    [Header("Item Holder Settings")]
    [SerializeField] private SOItem item;
    [Tooltip("Amount of item to give when opened, 0 is infinite.")]
    [SerializeField, Min(0)] private int amount = 1;
    [SerializeField, ReadOnly] private int itemsWhereTaken;
    
    private void TakeItem()
    {
        if (amount != 0 && itemsWhereTaken >= amount) return;

        if (PlayerInventory.Instance && PlayerInventory.Instance.TryAddItem(item))
        {
            itemsWhereTaken++;
        }

    }

    protected override void OnInteract()
    {
        TakeItem();
    }
}
