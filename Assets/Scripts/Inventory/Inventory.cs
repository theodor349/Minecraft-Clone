using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject InventoryUI;
    public InventorySlot[] Slots;
    public InventorySlot[] Amor;
    public InventorySlot[] CraftingInput;
    public InventorySlot CraftingInputOutput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InventoryUI.SetActive(!InventoryUI.activeSelf);
            if (InventoryUI.activeSelf)
                Cursor.lockState = CursorLockMode.None;
            else 
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public bool CanPickUp(Item item)
    {
        return GetFreeSlotIndex(item) != -1;
    }

    public int PickUp(Item item)
    {
        int index = GetFreeSlotIndex(item);
        if (index == -1)
            return 0;

        int amount = Slots[index].Accepts(item);
        if(amount >= item.StackSize)
        {
            Slots[index].PutItem(item);
            return item.StackSize;
        }
        else if(amount < item.StackSize)
        {
            Slots[index].PutItem(Item.Copy(item, amount));
            item.StackSize -= amount;
            return amount + PickUp(item);
        }
        else
        {
            Slots[index].PutItem(Item.Copy(item, amount));
            return amount;
        }
    }

    private int GetFreeSlotIndex(Item item)
    {
        int index = -1;
        for (int i = 0; i < Slots.Length; i++)
        {
            if (Slots[i].GetItem() != null && Slots[i].Accepts(item) > 0)
                return i;

            if (Slots[i].IsEmpty() && index == -1)
                index = i;
        }

        return index;
    }
}
