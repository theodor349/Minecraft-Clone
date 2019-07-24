using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBelt : MonoBehaviour
{
    public PlayerController Player;

    public Image[] ItemSlots;
    public Image SelectedItemSlot;

    private World world;
    private int selectedIndex;

    private void Start()
    {
        world = World.Instance;

        selectedIndex = Player.SelectedItemSlot;

        for (int i = 1; i < world.BlockTypes.Length; i++)
        {
            ItemSlots[i - 1].sprite = world.BlockTypes[i].Icon;
            ItemSlots[i - 1].color = Color.white;
        }
    }

    private void Update()
    {
        if (selectedIndex != Player.SelectedItemSlot)
            UpdateSelection();
    }

    private void UpdateSelection()
    {
        selectedIndex = Player.SelectedItemSlot;

        SelectedItemSlot.rectTransform.position = ItemSlots[selectedIndex].rectTransform.position;
    }

}
