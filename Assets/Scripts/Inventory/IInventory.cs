using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventory
{
    int Accepts(Item item, int index); // Will this ever be used? Is called when putting an item
    int PutItem(Item item, int index);

    int Accepts(Item item); // Will this ever be used? Is called when putting an item
    int PutItem(Item item);

    Item Read(int index);
    Item Take(int amount, int index);
    Item Remove(int index);
}
