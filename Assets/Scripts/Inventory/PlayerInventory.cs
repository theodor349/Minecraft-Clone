using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInventory : MonoBehaviour, IInventory
{
    public GameObject inventoryGameobject;
    public InventorySlot[] UISlots;
    public Slot[] inventory; // First 9 is tool-bar afterwards it is indexed from upper left

    private void Start()
    {
        inventory = new Slot[UISlots.Length];
        for (int i = 0; i < UISlots.Length; i++)
        {
            inventory[i] = new Slot();
        }

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            PutItem(new Item(BlockType.Sand, 32));

        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryGameobject.SetActive(!inventoryGameobject.activeSelf);
            MouseLook.SetMouseLock(!inventoryGameobject.activeSelf);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Item"))
        {
            var item = other.GetComponent<ItemGameobject>();
            item.Take(PutItem(Item.Copy(item.GetItem(), item.GetItem().StackSize)));
        }
    }

    public int Accepts(Item item, int index)
    {
        return inventory[index].MyItem == null ? item.MaxStackSize : inventory[index].MyItem.Accepts(item);
    }

    public int Accepts(Item item)
    {
        int amount = 0;
        for (int i = 0; i < inventory.Length; i++)
        {
            amount += inventory[i].MyItem.Accepts(item);
            if (amount > item.StackSize)
                return item.StackSize;
        }

        return amount;
    }

    public int PutItem(Item item, int index)
    {
        Item myItem = inventory[index].MyItem;

        if (myItem == null)
        {
            inventory[index].MyItem = item;
            return item.MaxStackSize;
        }

        int amount = myItem.Accepts(item);
        inventory[index].MyItem.StackSize += amount;
        return amount;
    }

    public int PutItem(Item item)
    {
        int i = GetFreeSlotIndex(item);
        if (i == -1)
            return 0;

        int amount = Accepts(item, i);
        if (amount >= item.StackSize)
        {
            PutItem(item, i);
            return item.StackSize;
        }
        else if (amount < item.StackSize)
        {
            PutItem(Item.Copy(item, amount), i);
            item.StackSize -= amount;
            return amount + PutItem(item);
        }
        else
        {
            PutItem(Item.Copy(item, amount), i);
            return amount;
        }
    }

    public Item Read(int index)
    {
        return inventory[index].MyItem;
    }

    public Item Take(int amount, int index)
    {
        Item item = Item.Copy(Read(index), amount);
        Read(index).StackSize -= amount;

        if (Read(index).StackSize == 0)
            inventory[index].RemoveItem();

        return item;
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

    public Item Remove(int index)
    {
        Item item = inventory[index].MyItem;
        inventory[index].RemoveItem();
        return item;
    }
}
