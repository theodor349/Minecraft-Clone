﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class World : MonoBehaviour
{
    public static World Instance;

    public Material Material;
    public Block[] BlockTypes;

    Chunk[,] chunks = new Chunk[BlockData.WorldWidthInChunks, BlockData.WorldWidthInChunks];

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("World: Multiple Worlds");
        Instance = this;

        BlockData.Init();
    }

    void Start()
    {
        for (int x = 0; x < BlockData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < BlockData.WorldWidthInChunks; z++)
            {
                chunks[x, z] = new Chunk(this, new Vector2Int(x, z), false);
            }
        }

        for (int x = 0; x < BlockData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < BlockData.WorldWidthInChunks; z++)
            {
                chunks[x, z].Draw();
            }
        }
    }
    
    public bool IsVoxelSolid(Vector3Int pos)
    {
        var chunkCoord = new Vector2Int(Mathf.FloorToInt(((float) pos.x) / BlockData.ChunkWidth), Mathf.FloorToInt(((float)pos.z) / BlockData.ChunkWidth));

        if (chunkCoord.x < 0 || chunkCoord.x >= BlockData.WorldWidthInChunks || chunkCoord.y < 0 || chunkCoord.y >= BlockData.WorldWidthInChunks)
            return false;

        return chunks[chunkCoord.x, chunkCoord.y].IsVoxelSolid(new Vector3Int(pos.x % BlockData.ChunkWidth, pos.y, pos.z % BlockData.ChunkWidth));
    }

    public byte GetBlockTypeAt(Vector3Int pos)
    {
        if (IsCoordOutsideWord(pos))
            return 0;

        Vector2Int chunk = GetChunkCoord(pos);
        return chunks[chunk.x, chunk.y].GetBlockTypeAt(GetBlockPosInChunk(pos));
    }

    public void EditBlock(Vector3Int pos, byte type)
    {
        if (IsCoordOutsideWord(pos))
            return;

        Vector2Int chunkCoord = GetChunkCoord(pos);
        chunks[chunkCoord.x, chunkCoord.y].EditBlock(GetBlockPosInChunk(pos), type);
    }

    internal void UpdateChunk(Vector2Int pos)
    {
        if (IsChunkOutsideWorld(pos))
            return;

        chunks[pos.x, pos.y].Update();
    }

    public void AddChunksToCollision(Vector2Int pos)
    {
        if (IsChunkOutsideWorld(pos))
            return;

        chunks[pos.x, pos.y].AddCollision();
    }

    public void RemoveChunksToCollision(Vector2Int pos)
    {
        if (IsChunkOutsideWorld(pos))
            return;

        chunks[pos.x, pos.y].RemoveCollision();
    }

    public static bool IsCoordOutsideWord(Vector3Int pos)
    {
        int worldWidth = BlockData.WorldWidthInChunks * BlockData.ChunkWidth;
        return (pos.x < 0 || pos.x >= worldWidth || pos.y < 0 || pos.y >= BlockData.ChunkHeight || pos.z < 0 || pos.z >= worldWidth);
    }

    public static bool IsChunkOutsideWorld(Vector2Int pos)
    {
        return (pos.x < 0 || pos.x >= BlockData.WorldWidthInChunks || pos.y < 0 || pos.y >= BlockData.WorldWidthInChunks);
    }

    public static Vector2Int GetChunkCoord(Vector3Int pos)
    {
        return new Vector2(pos.x / BlockData.ChunkWidth, pos.z / BlockData.ChunkWidth).Floor();
    }

    public static Vector3Int GetPosInChunk(Vector3Int pos)
    {
        return new Vector3Int((int)pos.x / BlockData.ChunkWidth, pos.y, pos.z / BlockData.ChunkWidth);
    }

    public static Vector3Int GetBlockPosInChunk(Vector3Int pos)
    {
        return new Vector3Int(pos.x % BlockData.ChunkWidth, pos.y, pos.z % BlockData.ChunkWidth);
    }

    #region World Generation
    public byte WorldGenGetBlockType(Vector3Int pos)
    {
        int terrainHeight = Mathf.FloorToInt(Get2DPerline(new Vector2(pos.x, pos.z), 0, 0.5f) * 32) + 8;
        if (pos.x == 6)
            return 2;

        if (pos.y == terrainHeight)
            return 4;
        else if (pos.y < terrainHeight && pos.y > terrainHeight - 4)
            return 3;
        else if (pos.y > terrainHeight)
            return 0;
        else if (pos.y == 0)
            return 1;
        else
            return 2;
    }

    public static float Get2DPerline(Vector2 position, float offset, float scale)
    {
        return Mathf.PerlinNoise(
            (position.x + 0.1f) / BlockData.ChunkWidth * scale + offset,
            (position.y + 0.1f) / BlockData.ChunkWidth * scale + offset
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
