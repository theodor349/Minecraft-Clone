using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject InventoryUI;
    public InventorySlot[] Slots;
    public InventorySlot[] Amor;
    public InventorySlot[] CraftingInput;
    public InventorySlot CraftingInputOutput;

    public InventorySlot[] ToolBar;
    public Image SelectedItemSlot;
    public int SelectedSlot = 0;

    private World world;


    private void Start()
    {
        world = World.Instance;

        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].SlotIndex = i;
            Slots[i].MyInventory = this;
        }
    }

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

        if (!InventoryUI.activeSelf)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                SelectedSlot++;
                UpdateToolBelt();
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                SelectedSlot--;
                UpdateToolBelt();
            }
        }
    }
    
    public void SlotChanged(int index, Item item)
    {
        if (index >= 9)
            return;

        ToolBar[index].RemoveItem();
        ToolBar[index].PutItem(item);
    }

    private void UpdateToolBelt()
    {
        if (SelectedSlot < 0)
            SelectedSlot = 8;
        else if (SelectedSlot == 9)
            SelectedSlot = 0;

        SelectedItemSlot.rectTransform.position = ToolBar[SelectedSlot].GetComponent<RectTransform>().position;
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
            if (Slots[i].ReadItem() != null && Slots[i].Accepts(item) > 0)
                return i;

            if (Slots[i].IsEmpty() && index == -1)
                index = i;
        }

        return index;
    }
}
