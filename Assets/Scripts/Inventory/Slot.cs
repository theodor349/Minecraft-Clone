using System;
using System.Collections.Generic;
using UnityEngine;

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
            SlotChanged(value);
        }
    }
    public Action<Item> SlotChanged;

    public Slot()
    {
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
