using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class World : MonoBehaviour
{
    public static World Instance;

    public Material Material;
    public Voxel[] VoxelTypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldWidthInChunks, VoxelData.WorldWidthInChunks];

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("World: Multiple Worlds");
        Instance = this;
    }

    void Start()
    {
        for (int x = 0; x < VoxelData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldWidthInChunks; z++)
            {
                chunks[x, z] = new Chunk(this, new Vector3Int(x, 0, z), false);
            }
        }

        for (int x = 0; x < VoxelData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldWidthInChunks; z++)
            {
                chunks[x, z].Draw();
            }
        }
    }
    
    public bool IsVoxelSolid(Vector3Int pos)
    {
        var chunkCoord = new Vector2Int(Mathf.FloorToInt(((float) pos.x) / VoxelData.ChunkWidth), Mathf.FloorToInt(((float)pos.z) / VoxelData.ChunkWidth));

        if (chunkCoord.x < 0 || chunkCoord.x >= VoxelData.WorldWidthInChunks || chunkCoord.y < 0 || chunkCoord.y >= VoxelData.WorldWidthInChunks)
            return false;

        return chunks[chunkCoord.x, chunkCoord.y].IsVoxelSolid(new Vector3Int(pos.x % VoxelData.ChunkWidth, pos.y, pos.z % VoxelData.ChunkWidth));
    }

    public byte GetBlockTypeAt(Vector3Int pos)
    {
        if (IsCoordOutsideWord(pos))
            return 0;

        Vector2Int chunk = new Vector2(pos.x / VoxelData.ChunkWidth, pos.z / VoxelData.ChunkWidth).Floor();
        return chunks[chunk.x, chunk.y].GetBlockTypeAt((new Vector3(pos.x % VoxelData.ChunkWidth, pos.y, pos.z % VoxelData.ChunkWidth)).Floor());
    }

    public bool IsCoordOutsideWord(Vector3Int pos)
    {
        int worldWidth = VoxelData.WorldWidthInChunks * VoxelData.ChunkWidth;
        return (pos.x < 0 || pos.x >= worldWidth || pos.y < 0 || pos.y >= VoxelData.ChunkHeight || pos.z < 0 || pos.z >= worldWidth);
    }

    #region World Generation
    public byte WorldGenGetBlockType(Vector3Int pos)
    {
        int terrainHeight = Mathf.FloorToInt(Get2DPerline(new Vector2(pos.x, pos.z), 0, 0.5f) * 32) + 8;
        if (pos.x == 6)
            return 1;

        if (pos.y == terrainHeight)
            return 3;
        else if (pos.y < terrainHeight && pos.y > terrainHeight - 4)
            return 2;
        else if (pos.y > terrainHeight)
            return 0;
        else
            return 1;
    }

    public static float Get2DPerline(Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise(
            (position.x + 0.1f) / VoxelData.ChunkWidth * scale + offset,
            (position.y + 0.1f) / VoxelData.ChunkWidth * scale + offset
            );
    }

    public static bool Get3DPerline(Vector3 position, float offset, float scale, float threshold)
    {
        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA + CB + CA) / 6f > threshold)
            return true;
        else
            return false;
    }
    #endregion
}
