using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Inventory : MonoBehaviour
{
    public GameObject InventoryUI;
    public InventorySlot[] UISlots;
    public InventorySlot[] UIArmor;
    public InventorySlot[] UICraftingInput;
    public InventorySlot UICraftingOutput;

    public InventorySlot[] UIToolBar;
    public Image SelectedItemSlot;
    public int SelectedSlot = 0;

    private Slot[] inventory;
    private Slot[] armor;
    private Slot[] craftingInput;
    private Slot craftingOutput;

    private void Start()
    {
        inventory = new Slot[UISlots.Length];
        armor = new Slot[UIArmor.Length];
        craftingInput = new Slot[UICraftingInput.Length];

        SetupCallbacks();
    }

    private void SetupCallbacks()
    {
        for (int i = 0; i < UISlots.Length; i++)
        {
            inventory[i] = new Slot();
            inventory[i].RegistreSlotChangedAction(UISlots[i].UpdateSlot);
            UISlots[i].PlayerInventory = this;
            UISlots[i].Index = i;
        }

        for (int i = 0; i < UIToolBar.Length; i++)
        {
            inventory[i].RegistreSlotChangedAction(UIToolBar[i].UpdateSlot);
            UIToolBar[i].PlayerInventory = this;
            UIToolBar[i].Index = i;
        }

        for (int i = 0; i < UIArmor.Length; i++)
        {
            armor[i] = new Slot();
            armor[i].RegistreSlotChangedAction(UIArmor[i].UpdateSlot);
            UIArmor[i].PlayerInventory = this;
            UIArmor[i].Index = UISlots.Length + i;
        }

        for (int i = 0; i < UICraftingInput.Length; i++)
        {
            craftingInput[i] = new Slot();
            craftingInput[i].RegistreSlotChangedAction(UICraftingInput[i].UpdateSlot);
            UICraftingInput[i].PlayerInventory = this;
            UICraftingInput[i].Index = UIArmor.Length + UISlots.Length + i;
        }

        craftingOutput = new Slot();
        craftingOutput.RegistreSlotChangedAction(UICraftingOutput.UpdateSlot);
        UICraftingOutput.PlayerInventory = this;
        UICraftingOutput.Index = UICraftingInput.Length + UIArmor.Length + UISlots.Length;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            if (PickUp(new Item(2, 1)) == 0)
                Debug.Log("Nothing PickUp");
        if (Input.GetKeyDown(KeyCode.Q))
            if (PickUp(new Item(3, 65)) == 0)
                Debug.Log("Nothing PickUp");

        if (Input.GetKeyDown(KeyCode.E))
        {
            InventoryUI.SetActive(!InventoryUI.activeSelf);
            if (InventoryUI.activeSelf)
                MouseLook.SetMouseLock(false);
            else
                MouseLook.SetMouseLock(true);
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

    private void UpdateToolBelt()
    {
        if (SelectedSlot < 0)
            SelectedSlot = 8;
        else if (SelectedSlot == 9)
            SelectedSlot = 0;

        SelectedItemSlot.rectTransform.position = UIToolBar[SelectedSlot].GetComponent<RectTransform>().position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            var item = other.GetComponent<ItemGameobject>();
            item.Take(PickUp(Item.Copy(item.GetItem(), item.GetItem().StackSize)));
        }
    }

    public bool CanPickUp(Item item)
    {
        return GetFreeSlotIndex(item) != -1;
    }

    public int PickUp(Item item)
    {
        int i = GetFreeSlotIndex(item);
        if (i == -1)
            return 0;

        int amount = Accepts(item, i);
        if(amount >= item.StackSize)
        {
            PutItem(item, i);
            return item.StackSize;
        }
        else if(amount < item.StackSize)
        {
            PutItem(Item.Copy(item, amount), i);
            item.StackSize -= amount;
            return amount + PickUp(item);
        }
        else
        {
            PutItem(Item.Copy(item, amount), i);
            return amount;
        }
    }

    public Item GetSlectedItem()
    {
        return inventory[SelectedSlot].MyItem;
    }

    private int GetFreeSlotIndex(Item item)
    {
        int index = -1;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].MyItem != null && Accepts(item, i) > 0)
                return i;

            if (inventory[i].MyItem == null && index == -1)
                index = i;
        }

        return index;
    }

    public int Accepts(Item item, int i)
    {
        if (inventory[i].MyItem == null)
            return item.MaxStackSize;

        if (item.BlockType != inventory[i].MyItem.BlockType)
            return 0;

        return inventory[i].MyItem.MaxStackSize - inventory[i].MyItem.StackSize;
    }

    public void PutItem(Item item, int i)
    {
        if (inventory[i].MyItem == null)
        {
            inventory[i].MyItem = item;
        }
        else
            inventory[i].MyItem.StackSize += item.StackSize;

    }

    public Item RemoveItem(int i)
    {
        Item item = inventory[i].MyItem;
        inventory[i].MyItem = null;

        return item;
    }

    public Item Take(int amount, int i)
    {
        Item item = Item.Copy(ReadItem(i), amount);
        ReadItem(i).StackSize -= amount;

        if(ReadItem(i).StackSize == 0)
            inventory[i].RemoveItem();

        return item;
    }

    public Item ReadItem(int i)
    {
        return inventory[i].MyItem;
    }
}
