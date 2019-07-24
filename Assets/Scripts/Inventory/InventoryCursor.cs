using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCursor : MonoBehaviour
{
    public static InventoryCursor instance;
    public GameObject Inventory;

    public RectTransform Pos;
    public Image MyImage;
    public Text MyText;

    private Item item;
    private bool isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        Pos.position = Input.mousePosition;

        if (isActive == Inventory.activeSelf)
            return;

        isActive = Inventory.activeSelf;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (isActive && item != null)
        {
            MyImage.sprite = item.Icon;
            MyImage.color = Color.white;
            MyText.text = item.StackSize == 1 ? "" : item.StackSize.ToString();
        }
        else
        {
            MyImage.color = new Color(0f, 0f, 0f, 0f);
            MyText.text = "";
        }
    }

    public void PutItem(Item item)
    {
        this.item = item;

        UpdateVisuals();
    }

    public Item Take(int amount)
    {
        item.StackSize -= amount;

        UpdateVisuals();
        return Item.Copy(item, amount);
    }

    public Item GetItem()
    {
        Item i = item;
        item = null;

        UpdateVisuals();
        return i;
    }

    public Item ReadItem()
    {
        return item;
    }
}
