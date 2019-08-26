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
    private Vector3 offset;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        UpdateVisuals();
    }

    private void Update()
    {
        Pos.position = Input.mousePosition;

        if (isActive == Inventory.activeSelf)
            return;

        isActive = Inventory.activeSelf;
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

    public Item ReadItem()
    {
        return item;
    }

    public Item GetItem()
    {
        Item i = item;
        item = null;

        UpdateVisuals();
        return i;
    }

    public void PutItem(Item item)
    {
        this.item = item;

        UpdateVisuals();
    }

    public Item Take(int amount)
    {
        item.StackSize -= amount;
        Item i = Item.Copy(item, amount);

        if (item.StackSize < 1)
            item = null;

        UpdateVisuals();
        return i;
    }
}