using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image ItemImage;
    public Text ItemAmountText;
    public Inventory PlayerInventory;
    public int Index;

    private InventoryCursor cursor;

    private void Start()
    {
        cursor = InventoryCursor.instance;
    }

    public void OnClick()
    {
        Item other = cursor.ReadItem();

        if (other == null)
        {
            cursor.PutItem(PlayerInventory.RemoveItem(Index));
            return;
        }

        if (PlayerInventory.ReadItem(Index) == null)
        {
            PlayerInventory.PutItem(cursor.GetItem(), Index);
            return;
        }

        if (PlayerInventory.ReadItem(Index).BlockType != other.BlockType)
        {
            cursor.PutItem(PlayerInventory.RemoveItem(Index));
            PlayerInventory.PutItem(other, Index);
            return;
        }

        int amount = PlayerInventory.Accepts(other, Index);
        amount = Mathf.Min(amount, other.StackSize);

        if (amount == other.StackSize)
        {
            PlayerInventory.PutItem(cursor.GetItem(), Index);
            return;
        }

        PlayerInventory.PutItem(cursor.Take(amount), Index);
    }

    public void UpdateSlot(Item item)
    {
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
