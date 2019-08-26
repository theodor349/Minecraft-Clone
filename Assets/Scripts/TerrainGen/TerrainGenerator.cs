﻿using UnityEngine;

public static class TerrainGenerator
{
    public static BlockType WorldGenGetBlockType(Vector3Int pos)
    {
        int terrainHeight = Mathf.FloorToInt(Get2DPerline(new Vector2(pos.x, pos.z), 0, 0.5f) * 32) + 8;
        if (pos.x == 6)
            return BlockType.Log;

        if (pos.y == terrainHeight)
            return BlockType.Grass;
        else if (pos.y < terrainHeight && pos.y > terrainHeight - 4)
            return BlockType.Dirt;
        else if (pos.y > terrainHeight)
            return BlockType.Air;
        else if (pos.y == 0)
            return BlockType.Bedrock;
        else
            return BlockType.Stone;
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

        return (AB + BC + AC + BA + CB + CA) / 6f > threshold;
    }
}