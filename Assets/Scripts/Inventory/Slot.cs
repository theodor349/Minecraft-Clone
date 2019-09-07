using System;

public class Slot
{
    private Item myItem;

    public Item MyItem {
        get {
            return myItem;
        }
        set {
            if (myItem != null)
                myItem.UnregistreItemChanged(SlotChanged);
            myItem = value;
            if (myItem != null)
                myItem.RegistreItemChangedAction(SlotChanged);
            SlotChanged(myItem);
        }
    }

    public Action<Item> SlotChanged;

    public Slot()
    {
    }

    public void RemoveItem()
    {
        MyItem = null;
    }

    public void RegistreSlotChangedAction(Action<Item> callback)
    {
        SlotChanged += callback;
    }

    public void UnregistreSlotChanged(Action<Item> callback)
    {
        SlotChanged -= callback;
    }
}