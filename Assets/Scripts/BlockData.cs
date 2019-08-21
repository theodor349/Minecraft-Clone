using UnityEngine;

public static class BlockData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 256; // 256; // Cant draw the full chunk border
    public static readonly int WorldWidthInChunks = 2; // int.MaxValue; // Is equal to 1714 times the circumference of earth 

    public static readonly int TextureAtlasBlockWidth = 16;
    public static float NormalizedTextureWidth;
    public static float PaddingWidth;

    public static void Init()
    {
        NormalizedTextureWidth = 1f / (float)TextureAtlasBlockWidth;
        PaddingWidth = NormalizedTextureWidth / 10f;
    }

    public static readonly Vector3[] Vertices = new Vector3[]
    {
        new Vector3(0,0,0), // 0
        new Vector3(1,0,0), // 1
        new Vector3(1,1,0), // 2
        new Vector3(0,1,0), // 3 
        new Vector3(0,0,1), // 4
        new Vector3(1,0,1), // 5 
        new Vector3(1,1,1), // 6
        new Vector3(0,1,1), // 7 
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

    //public static readonly int[,] RotateFace = new int[,]
    //{
    //    { 0, 1, 2, 3, 4, 5 }, // Nothing
    //    { 0, 1, 2, 3, 4, 5 }, // North
    //    { 1, 0, 2, 3, 5, 4 }, // South
    //    { 0, 1, 2, 3, 4, 5 }, // Top
    //    { 0, 1, 2, 3, 4, 5 }, // Buttom
    //    { 5, 5, 2, 3, 5, 5 }, // West
    //    { 0, 0, 2, 3, 0, 0 }, // East
    //};

    public static readonly int[,] Triangles = new int[,]
    {
        { 0,3,1,2 }, // Back face
        { 5,6,4,7 }, // Front face
        { 3,7,2,6 }, // Top face
        { 1,5,0,4 }, // Buttom face
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