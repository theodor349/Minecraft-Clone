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

    public void RightClick()
    {
        Item other = cursor.ReadItem();
        Item item = PlayerInventory.ReadItem(Index);

        if (other == null && item == null)
            return;

        if (other == null && item != null)
        {
            int amount = Mathf.FloorToInt(PlayerInventory.ReadItem(Index).StackSize * 0.5f);
            if (amount == 0)
                cursor.PutItem(PlayerInventory.RemoveItem(Index));
            else
                cursor.PutItem(PlayerInventory.Take(amount, Index));

            return;
        }

        if (PlayerInventory.Accepts(other, Index) > 0)
        {
            PlayerInventory.PutItem(cursor.Take(1), Index);
            return;
        }
    }

    public void LeftClick()
    {
        Item other = cursor.ReadItem();
        Item item = PlayerInventory.ReadItem(Index);

        if (other == null)
        {
            cursor.PutItem(PlayerInventory.RemoveItem(Index));
            return;
        }

        if (item == null)
        {
            PlayerInventory.PutItem(cursor.GetItem(), Index);
            return;
        }

        if (item.BlockType != other.BlockType)
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