using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image ItemImage;
    public Text ItemAmountText;
    public IInventory PlayerInventory;
    public int Index;

    private InventoryCursor cursor;

    private void Start()
    {
        cursor = InventoryCursor.instance;
    }

    public void RightClick()
    {
        Item other = cursor.ReadItem();
        Item item = PlayerInventory.Read(Index);

        if (other == null && item == null)
            return;

        if (other == null && item != null)
        {
            int amount = Mathf.FloorToInt(PlayerInventory.Read(Index).StackSize * 0.5f);
            if (amount == 0)
                cursor.PutItem(PlayerInventory.Remove(Index));
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
        Item c = cursor.ReadItem();
        Item s = PlayerInventory.Read(Index);

        if (c == null)
        {
            cursor.PutItem(PlayerInventory.Remove(Index));
            return;
        }

        if (s == null)
        {
            PlayerInventory.PutItem(cursor.GetItem(), Index);
            return;
        }

        if (s.BlockType != c.BlockType)
        {
            cursor.PutItem(PlayerInventory.Remove(Index));
            PlayerInventory.PutItem(c, Index);
            return;
        }

        int amount = PlayerInventory.Accepts(c, Index);
        amount = Mathf.Min(amount, c.StackSize);

        if (amount == c.StackSize)
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