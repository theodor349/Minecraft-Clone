using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image ItemImage;
    public Text ItemAmountText;

    private Item item;

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

    public Item GetItem()
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
        if(item == null)
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
