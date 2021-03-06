﻿using System;
using UnityEngine;

public class Item
{
    public BlockType BlockType;
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
            if (ItemChanged != null)
                ItemChanged(this);
        }
    }

    public int Accepts(Item item)
    {
        if (item.BlockType != BlockType)
            return 0;
        return Math.Min(item.stackSize, MaxStackSize - stackSize);
    }

    // Just initialize with a normal block
    public Item(BlockType blockType, int amount)
    {
        BlockType = blockType;
        Icon = World.Instance.BlockTypes[(int)blockType].Icon;
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