using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 64; // 256; // Cant draw the full chunk border
    public static readonly int WorldWidthInChunks = 10;// int.MaxValue; // Is equal to 1714 times earth circumference 

    public static readonly int TextureAtlasBlockWidth = 16;
    public static readonly float NormalizedTextureWidth = 0.0625f;

    public static readonly Vector3[] Vertices = new Vector3[]
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(1,1,0),
        new Vector3(0,1,0),
        new Vector3(0,0,1),
        new Vector3(1,0,1),
        new Vector3(1,1,1),
        new Vector3(0,1,1),
    };

    public static readonly Vector3Int[] Neighbors = new Vector3Int[]
    {
        new Vector3Int( 0, 0,-1),
        new Vector3Int( 0, 0, 1),
        new Vector3Int( 0, 1, 0),
        new Vector3Int( 0,-1, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int( 1, 0, 0),
    };

    public static readonly int[,] Triangles = new int[,]
    {
        { 0,3,1,2 }, // Back face
        { 5,6,4,7 }, // Front face
        { 3,7,2,6 }, // Top face
        { 1,5,0,4 }, // Bottom face
        { 4,7,0,3 }, // Left face
        { 1,2,5,6 }, // Right face
    };

    public static readonly Vector2[] Uvs = new Vector2[]
    {
        new Vector2(0,0),
        new Vector2(0,1),
        new Vector2(1,0),
        new Vector2(1,1),
    };

}