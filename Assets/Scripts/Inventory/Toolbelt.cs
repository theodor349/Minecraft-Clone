using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInventory))]
public class Toolbelt : MonoBehaviour
{
    public InventorySlot[] UIToolBar;
    public Image SelectedItemSlot;
    public int SelectedSlot = 0;

    private GameObject inventoryGameobject;
    private PlayerInventory playerInventory;

    private void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        inventoryGameobject = playerInventory.inventoryGameobject;

        for (int i = 0; i < UIToolBar.Length; i++)
        {
            playerInventory.inventory[i].RegistreSlotChangedAction(UIToolBar[i].UpdateSlot);
            UIToolBar[i].PlayerInventory = playerInventory;
            UIToolBar[i].Index = i;
        }
    }

    private void Update()
    {
        UpdateToolBelt();
    }

    private void UpdateToolBelt()
    {
        if (!inventoryGameobject.activeSelf)
        {
            if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                SelectedSlot++;
                SetSelectedIndex();
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                SelectedSlot--;
                SetSelectedIndex();
            }
        }
    }

    private void SetSelectedIndex()
    {
        if (SelectedSlot < 0)
            SelectedSlot = 8;
        else if (SelectedSlot == 9)
            SelectedSlot = 0;

        SelectedItemSlot.rectTransform.position = UIToolBar[SelectedSlot].GetComponent<RectTransform>().position;
    }

    public Item GetSlectedItem()
    {
        return playerInventory.inventory[SelectedSlot].MyItem;
    }

    public void Take(int amount)
    {
        playerInventory.Take(amount, SelectedSlot);
    }
}