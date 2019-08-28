using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    public Material Mat;
    public Material TransparentMat;
    public PhysicMaterial PhysicsMat;
    public Transform ItemParent;
    public Block[] BlockTypes;

    private Chunk[,] chunks = new Chunk[BlockData.WorldWidthInChunks, BlockData.WorldWidthInChunks];
    private Queue<IConstruction> constructions = new Queue<IConstruction>();

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("World: Multiple Worlds");
        World.Instance = this;

        InitializeStaticObjcts();
        OrganizeBlockTypes();
    }

    private void InitializeStaticObjcts()
    {
        BlockData.Init();
        Structures.Init();
    }

    private void OrganizeBlockTypes()
    {
        var blockTypes = Enum.GetValues(typeof(BlockType));
        var temp = new Block[blockTypes.Length];

        for (int i = 0; i < blockTypes.Length; i++)
        {
            for (int n = 0; n < BlockTypes.Length; n++)
            {
                if (BlockTypes[n].name.Equals(Enum.GetName(typeof(BlockType), i)))
                {
                    temp[i] = BlockTypes[n];
                    break;
                }
            }
        }

        BlockTypes = temp;
    }

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = 0; x < BlockData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < BlockData.WorldWidthInChunks; z++)
            {
                chunks[x, z] = new Chunk(this, new Vector2Int(x, z), false);
            }
        }

        while(constructions.Count > 0)
        {
            constructions.Dequeue().Construct();
        }

        for (int x = 0; x < BlockData.WorldWidthInChunks; x++)
        {
            for (int z = 0; z < BlockData.WorldWidthInChunks; z++)
            {
                chunks[x, z].Draw();
            }
        }
    }

    public void BreakBlock(Vector3Int pos)
    {
        if (IsCoordOutsideWord(pos))
            return;

        SpawnItem(pos);

        EditBlock(pos, 0);
    }

    public void EditBlock(Vector3Int pos, BlockType type, Direction rotation = Direction.Nothing)
    {
        if (IsCoordOutsideWord(pos))
            return;

        Vector2Int chunkCoord = GetChunkCoord(pos);
        chunks[chunkCoord.x, chunkCoord.y].EditBlock(GetBlockPosInChunk(pos), type, rotation);
    }

    public void AddContruction(IConstruction construction)
    {
        constructions.Enqueue(construction);
    }

    private void SpawnItem(Vector3Int pos)
    {
        var item = GetBlock(GetBlockTypeAt(pos)).Break(0);
        if (item == (byte)BlockType.Air)
            return;
        new GameObject().AddComponent<ItemGameobject>().Initialize(pos, new Item(item, 1));
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

    public bool IsVoxelSolid(Vector3Int pos)
    {
        var chunkCoord = new Vector2Int(Mathf.FloorToInt(((float)pos.x) / BlockData.ChunkWidth), Mathf.FloorToInt(((float)pos.z) / BlockData.ChunkWidth));

        if (chunkCoord.x < 0 || chunkCoord.x >= BlockData.WorldWidthInChunks || chunkCoord.y < 0 || chunkCoord.y >= BlockData.WorldWidthInChunks)
            return false;

        return chunks[chunkCoord.x, chunkCoord.y].IsVoxelSolid(new Vector3Int(pos.x % BlockData.ChunkWidth, pos.y, pos.z % BlockData.ChunkWidth));
    }

    public static Vector2Int GetChunkCoord(Vector3Int pos)
    {
        return new Vector2(pos.x / BlockData.ChunkWidth, pos.z / BlockData.ChunkWidth).Floor();
    }

    public static Vector3Int GetBlockPosInChunk(Vector3Int pos)
    {
        return new Vector3Int(pos.x % BlockData.ChunkWidth, pos.y, pos.z % BlockData.ChunkWidth);
    }

    public Block GetBlock(BlockType type)
    {
        return BlockTypes[(byte)type];
    }

    public BlockType GetBlockTypeAt(Vector3Int pos)
    {
        if (IsCoordOutsideWord(pos))
            return 0;

        Vector2Int chunk = GetChunkCoord(pos);
        return chunks[chunk.x, chunk.y].GetBlockTypeAt(GetBlockPosInChunk(pos));
    }
}