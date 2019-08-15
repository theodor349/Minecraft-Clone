using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoOverlay : MonoBehaviour
{
    public World World;
    public PlayerController Player;
    public Text BlockTypeText;
    public Text BlockPosText;

    private void Update()
    {
        SetBlockText();
    }

    private void SetBlockText()
    {
        if (Player.HighlightBlockPos.x == -1)
        {
            BlockTypeText.text = "BlockType:";
            BlockPosText.text = "BlockPos: ";
        }
        else
        {
            BlockTypeText.text = "BlockType: " + World.GetBlock(World.GetBlockTypeAt(Player.HighlightBlockPos)).name;
            BlockPosText.text = "Position: " + Player.HighlightBlockPos;
        }
    }
}
