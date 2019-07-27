using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public byte BlockType;
    public Sprite Icon;
    public int MaxStackSize;
    public Action<Item> ItemChanged;

    private int stackSize;
    public int StackSize {
        get {
            return stackSize;
        }
        set {
            stackSize = value;
            if(ItemChanged != null)
                ItemChanged(this);
        }
    }

    // Just initialize with a normal block
    public Item(byte blockType, int amount)
    {
        BlockType = blockType;
        Icon = World.Instance.BlockTypes[blockType].Icon;
        StackSize = amount;
        MaxStackSize = 64;
    }

    internal static Item Copy(Item item, int amount)
    {
        return new Item(item.BlockType, amount);
    }

    public void RegistreItemChangedAction(Action<Item> callback)
    {
        ItemChanged += callback;
    }

    public void UnregistreItemChanged(Action<Item> callback)
    {
        ItemChanged -= callback;
    }
}
