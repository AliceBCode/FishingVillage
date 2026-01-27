


using DNExtensions;
using UnityEngine;


public class ItemHolder : Interactable
{

    [Header("Item Holder Settings")]
    [SerializeField] private SOItem item;
    [SerializeField] private bool disableAfterPickup;
    [Tooltip("Amount of item to give when taken, 0 is infinite.")]
    [SerializeField, Min(0)] private int amount = 1;
    [SerializeField, ReadOnly] private int itemsWhereTaken;
    
    private void TakeItem()
    {
        if (amount != 0 && itemsWhereTaken >= amount) return;

        if (PlayerInventory.Instance && PlayerInventory.Instance.TryAddItem(item))
        {
            itemsWhereTaken++;
            
            if (disableAfterPickup) gameObject.SetActive(false);
        }

    }

    protected override void OnInteract()
    {
        TakeItem();
    }
}
