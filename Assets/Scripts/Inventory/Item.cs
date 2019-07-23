using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public byte BlockType;
    public Sprite Icon;
    public int StackSize;
    public int MaxStackSize;

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
}
