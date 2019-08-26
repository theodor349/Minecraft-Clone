using UnityEngine;

public interface IStructure
{
    void BuildStructure(World world, Vector3Int pos, int ground);
}

public class Tree : IStructure
{
    public void BuildStructure(World world, Vector3Int pos, int ground)
    {
        int h = 6;
        for (int i = 0; i < h; i++)
        {
            world.EditBlock(pos + new Vector3Int(0, ground + i, 0), BlockType.Log);
        }

        //for (int i = 0; i < 4; i++)
        //{

        //    if(i < 1)
        //        world.EditBlock(pos + new Vector3Int(-2, ground + l + i, -2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-1, ground + l + i, -2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(0, ground + l + i, -2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(1, ground + l + i, -2), BlockType.Leaves);
        //    if (i < 1)
        //        world.EditBlock(pos + new Vector3Int(2, ground + l + i, -2), BlockType.Leaves);

        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-2, ground + l + i, -1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-1, ground + l + i, -1), BlockType.Leaves);
        //    world.EditBlock(pos + new Vector3Int(0, ground + l + i, -1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(1, ground + l + i, -1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(2, ground + l + i, -1), BlockType.Leaves);

        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-2, ground + l + i, 0), BlockType.Leaves);
        //    world.EditBlock(pos + new Vector3Int(-1, ground + l + i, 0), BlockType.Leaves);
        //    world.EditBlock(pos + new Vector3Int(1, ground + l + i, 0), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(2, ground + l + i, 0), BlockType.Leaves);

        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-2, ground + l + i, 1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-1, ground + l+ i, 1), BlockType.Leaves);
        //    world.EditBlock(pos + new Vector3Int(0, ground + l + i, 1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(1, ground + l + i, 1), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(2, ground + l + i, 1), BlockType.Leaves);

        //    if (i < 1)
        //        world.EditBlock(pos + new Vector3Int(-2, ground + l + i, 2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(-1, ground + l + i, 2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(0, ground + l + i, 2), BlockType.Leaves);
        //    if (i < 2)
        //        world.EditBlock(pos + new Vector3Int(1, ground + l + i, 2), BlockType.Leaves);
        //    if (i < 1)
        //        world.EditBlock(pos + new Vector3Int(2, ground + l + i, 2), BlockType.Leaves);

        //    world.EditBlock(pos + new Vector3Int(0, ground + h, 0), BlockType.Leaves);
        //}
    }
}