using System;
using UnityEngine;

public class Tree : IStructure
{
    public void Build(World world, Vector3Int pos)
    {
        int height = 5;
        for (int i = 0; i < height; i++)
        {
            world.EditBlock(pos + new Vector3Int(0, i, 0), BlockType.Log);
        }

        for (int h = 0; h < 2; h++)
        {
            for (int i = 0; i < 5; i++)
            {
                world.EditBlock(pos + new Vector3Int(-2, 3 + h, i - 2), BlockType.Leaves);
                world.EditBlock(pos + new Vector3Int(2, 3 + h, i - 2), BlockType.Leaves);

                world.EditBlock(pos + new Vector3Int(-1, 3 + h, i - 2), BlockType.Leaves);
                world.EditBlock(pos + new Vector3Int(1, 3 + h, i - 2), BlockType.Leaves);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            world.EditBlock(pos + new Vector3Int(0, 3 + i, 2), BlockType.Leaves);
            world.EditBlock(pos + new Vector3Int(0, 3 + i, 1), BlockType.Leaves);

            world.EditBlock(pos + new Vector3Int(0, 3 + i, -2), BlockType.Leaves);
            world.EditBlock(pos + new Vector3Int(0, 3 + i, -1), BlockType.Leaves);
        }

        for (int i = 0; i < 3; i++)
        {
            world.EditBlock(pos + new Vector3Int(-1, 5, i - 1), BlockType.Leaves);
            world.EditBlock(pos + new Vector3Int(0, 5, i - 1), BlockType.Leaves);
            world.EditBlock(pos + new Vector3Int(1, 5, i - 1), BlockType.Leaves);
        }

        for (int i = 0; i < 3; i++)
        {
            world.EditBlock(pos + new Vector3Int(0, 6, i - 1), BlockType.Leaves);
            world.EditBlock(pos + new Vector3Int(i - 1, 6, 0), BlockType.Leaves);
        }
    }
}