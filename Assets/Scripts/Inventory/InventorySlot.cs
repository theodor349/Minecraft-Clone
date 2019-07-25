using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image ItemImage;
    public Text ItemAmountText;
    public InventoryCursor cursor;
    public int SlotIndex;
    public Inventory MyInventory;
    public bool IsToolBar;

    private Item item;

    private void Start()
    {
        cursor = InventoryCursor.instance;
    }

    public bool IsEmpty()
    {
        return item == null;
    }

    public int Accepts(Item item)
    {
        if (this.item == null)
            return item.MaxStackSize;

        if (item.BlockType != this.item.BlockType)
            return 0;

        return this.item.MaxStackSize - this.item.StackSize;
    }

    public void OnClick()
    {
        Item other = cursor.ReadItem();

        if (other == null)
        {
            cursor.PutItem(RemoveItem());
            return;
        }

        if(item == null)
        {
            PutItem(cursor.GetItem());
            return;
        }

        if (item.BlockType != other.BlockType)
        {
            cursor.PutItem(RemoveItem());
            PutItem(other);
            return;
        }

        int amount = Accepts(other);
        amount = Mathf.Min(amount, other.StackSize);

        if (amount == other.StackSize)
        {
            PutItem(cursor.GetItem());
            return;
        }

        PutItem(cursor.Take(amount));
    }

    public Item ReadItem()
    {
        return item;
    }

    public void PutItem(Item item)
    {
        if (this.item == null)
            this.item = item;
        else
            this.item.StackSize += item.StackSize;

        UpdateSlot();
    }

    public Item RemoveItem()
    {
        Item i = item;
        item = null;

        UpdateSlot();
        return i;
    }

    private void UpdateSlot()
    {
        if(!IsToolBar)
            MyInventory.SlotChanged(SlotIndex, item);

        if (item == null)
        {
            ItemImage.color = new Color(255f, 255f, 255f, 0f);
            ItemAmountText.text = "";
        }
        else
        {
            ItemImage.sprite = item.Icon;
            ItemImage.color = new Color(255f, 255f, 255f, 255f);
            ItemAmountText.text = item.StackSize == 1 ? "" : item.StackSize.ToString();
        }
    }
}
