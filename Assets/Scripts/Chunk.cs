using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk
{
    private int collisionPointers = 0;

    private GameObject obj;
    private Vector3Int chunkPos;
    private Vector2Int chunkCoord;
    private MeshRenderer rendere;
    private MeshFilter filter;
    private MeshCollider collider;

    private World world;
    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> tries = new List<int>();
    private List<int> transparentTries = new List<int>();

    private byte[,,] blocks;
    private Dictionary<Vector3Int, Direction> rotations = new Dictionary<Vector3Int, Direction>();

    // Drawing
    private int vertexIndex = 0;

    public Chunk(World world, Vector2Int pos, bool drawOnInit = true)
    {
        this.world = world;
        chunkCoord = pos;
        chunkPos = new Vector3Int(pos.x * BlockData.ChunkWidth, 0, pos.y * BlockData.ChunkWidth);

        obj = new GameObject();
        obj.layer = 8;
        obj.transform.position = chunkPos;
        obj.transform.SetParent(world.transform);
        obj.name = "Chunk " + chunkCoord.x + "_" + chunkCoord.y;
        obj.tag = "Chunk";

        rendere = obj.AddComponent<MeshRenderer>();
        filter = obj.AddComponent<MeshFilter>();
        collider = obj.AddComponent<MeshCollider>();
        collider.enabled = false;

        rendere.materials = new Material[]
        {
            world.Mat,
            world.TransparentMat
        };

        GenerateChunk();
        if (drawOnInit)
            Draw();
    }

    public bool IsVoxelSolid(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= BlockData.ChunkWidth || pos.z < 0 || pos.z >= BlockData.ChunkWidth)
            return world.IsVoxelSolid(pos + chunkPos);
        if (pos.y < 0)
            return true;
        if (pos.y >= BlockData.ChunkHeight)
            return false;

        return blocks[pos.x, pos.y, pos.z] != 0;
    }

    private void GenerateChunk()
    {
        blocks = new byte[BlockData.ChunkWidth, BlockData.ChunkHeight, BlockData.ChunkWidth];

        for (int x = 0; x < BlockData.ChunkWidth; x++)
        {
            for (int y = 0; y < BlockData.ChunkHeight; y++)
            {
                for (int z = 0; z < BlockData.ChunkWidth; z++)
                {
                    blocks[x, y, z] = (byte)TerrainGenerator.WorldGenGetBlockType(world, new Vector3Int(x + chunkPos.x, y, z + chunkPos.z));
                }
            }
        }
    }

    public BlockType GetBlockTypeAt(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= BlockData.ChunkWidth || pos.z < 0 || pos.z >= BlockData.ChunkWidth || pos.y < 0 || pos.y >= BlockData.ChunkHeight)
            return world.GetBlockTypeAt(pos + chunkPos);

        return (BlockType)blocks[pos.x, pos.y, pos.z];
    }

    public void EditBlock(Vector3Int pos, BlockType type, Direction rotaion)
    {
        bool changeBlock = true;

        rotations.Remove(pos);
        if (rotaion != Direction.Nothing)
            rotations.Add(pos, rotaion);

        if (world.GetBlock(type).UseGravity)
        {
            changeBlock = ApplyGravity(pos, type, rotaion);
        }

        if (changeBlock)
        {
            blocks[pos.x, pos.y, pos.z] = (byte)type;
            world.AddChunkChanged(this);
        }

        var edge = IsOnEdgeOfChunk(pos);
        if (edge == Direction.North)
            world.UpdateChunk(chunkCoord + new Vector2Int(0, 1));
        else if (edge == Direction.South)
            world.UpdateChunk(chunkCoord + new Vector2Int(0, -1));
        else if (edge == Direction.West)
            world.UpdateChunk(chunkCoord + new Vector2Int(-1, 0));
        else if (edge == Direction.East)
            world.UpdateChunk(chunkCoord + new Vector2Int(1, 0));
    }

    private bool ApplyGravity(Vector3Int pos, BlockType type, Direction rotation)
    {
        var block = GetBlockTypeAt(new Vector3Int(pos.x, pos.y - 1, pos.z));
        if (block != BlockType.Air)
            return true;

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var rb = go.AddComponent<Rigidbody>();
        rb.constraints =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;
        go.AddComponent<BoxCollider>();
        go.transform.SetParent(obj.transform);
        go.transform.position = pos + new Vector3(0.5f, 0, 0.5f) + chunkPos;
        var fall = go.AddComponent<FallingBlock>();
        fall.Block = world.GetBlock(type);
        fall.Type = type;
        fall.Rotation = rotation;

        return false;
    }

    public void AddCollision()
    {
        if (collisionPointers == 0)
            collider.enabled = true;
        collisionPointers++;
    }

    public void RemoveCollision()
    {
        collisionPointers--;
        if (collisionPointers == 0)
            collider.enabled = false;
    }

    public void Update()
    {
        Draw();
    }

    private Direction IsOnEdgeOfChunk(Vector3Int pos)
    {
        if (pos.x == 0)
            return Direction.West;
        else if (pos.x == BlockData.ChunkWidth - 1)
            return Direction.East;
        else if (pos.z == 0)
            return Direction.South;
        else if (pos.z == BlockData.ChunkWidth - 1)
            return Direction.North;

        return Direction.Nothing;
    }

    #region Drawing

    public void Draw()
    {
        DrawChunk();
    }

    private void DrawChunk()
    {
        ClearMesh();
        mesh = new Mesh();
        mesh.subMeshCount = 2;

        for (int x = 0; x < BlockData.ChunkWidth; x++)
        {
            for (int y = 0; y < BlockData.ChunkHeight; y++)
            {
                for (int z = 0; z < BlockData.ChunkWidth; z++)
                {
                    DrawVoxel(new Vector3Int(x, y, z));
                }
            }
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tries.ToArray(), 0);
        mesh.SetTriangles(transparentTries.ToArray(), 1);
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        filter.mesh = mesh;
        if (collisionPointers > 0)
            collider.sharedMesh = mesh;
        collider.sharedMesh = mesh;
    }

    private void ClearMesh()
    {
        vertexIndex = 0;
        verts.Clear();
        tries.Clear();
        transparentTries.Clear();
        uvs.Clear();
    }

    private void DrawVoxel(Vector3Int pos)
    {
        if (!IsVoxelSolid(pos))
            return;

        var rotation = Direction.Nothing;
        if (rotations.ContainsKey(pos))
            rotation = rotations[pos];

        for (int face = 0; face < 6; face++)
        {
            if (ShouldDrawVoxel(pos, face, rotation))
                continue;

            var block = world.BlockTypes[blocks[pos.x, pos.y, pos.z]];

            AddVertices(pos, face, rotation);
            AddTriangles(block.IsTransparent);
            AddTexture(block.GetFaceTexture((Face)face, rotation));

            vertexIndex += 4;
        }
    }

    private bool ShouldDrawVoxel(Vector3Int pos, int face, Direction rotation)
    {
        bool shoulDraw = IsVoxelSolid(pos + BlockData.Neighbors[face]);
        if (shoulDraw)
            shoulDraw = !world.GetBlock(GetBlockTypeAt(pos + BlockData.Neighbors[face])).IsTransparent;

        return shoulDraw;
    }

    private void AddVertices(Vector3Int pos, int face, Direction rotation)
    {
        if (face == 2 || face == 3)
        {
            verts.Add(RotateVertices(pos + BlockData.Vertices[BlockData.Triangles[face, 0]].Floor(), rotation, pos));
            verts.Add(RotateVertices(pos + BlockData.Vertices[BlockData.Triangles[face, 1]].Floor(), rotation, pos));
            verts.Add(RotateVertices(pos + BlockData.Vertices[BlockData.Triangles[face, 2]].Floor(), rotation, pos));
            verts.Add(RotateVertices(pos + BlockData.Vertices[BlockData.Triangles[face, 3]].Floor(), rotation, pos));
        }
        else
        {
            verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 0]]);
            verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 1]]);
            verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 2]]);
            verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 3]]);
        }
    }

    private Vector3Int RotateVertices(Vector3Int v, Direction rotation, Vector3Int pos)
    {
        if (rotation == Direction.North || rotation == Direction.Nothing)
            return v;

        Vector3 center = new Vector3(pos.x + 0.5f, pos.y, pos.z + 0.5f);

        int times = 0;
        if (rotation == Direction.South)
            times = 2;
        else if (rotation == Direction.West)
            times = 1;
        else if (rotation == Direction.East)
            times = 3;

        for (int i = 0; i < times; i++)
        {
            v = RotateVertex90(v, center);
        }

        return v;
    }

    private Vector3Int RotateVertex90(Vector3Int v, Vector3 center)
    {
        if (v.x < center.x)
        {
            if (v.z < center.z)
                return new Vector3Int(v.x + 1, v.y, v.z);
            else
                return new Vector3Int(v.x, v.y, v.z - 1);
        }
        else
        {
            if (v.z < center.z)
                return new Vector3Int(v.x, v.y, v.z + 1);
            else
                return new Vector3Int(v.x - 1, v.y, v.z);
        }
    }

    private void AddTriangles(bool isTransparent)
    {
        if (isTransparent)
        {
            transparentTries.Add(vertexIndex + 0);
            transparentTries.Add(vertexIndex + 1);
            transparentTries.Add(vertexIndex + 2);
            transparentTries.Add(vertexIndex + 2);
            transparentTries.Add(vertexIndex + 1);
            transparentTries.Add(vertexIndex + 3);
        }
        else
        {
            tries.Add(vertexIndex + 0);
            tries.Add(vertexIndex + 1);
            tries.Add(vertexIndex + 2);
            tries.Add(vertexIndex + 2);
            tries.Add(vertexIndex + 1);
            tries.Add(vertexIndex + 3);
        }
    }

    private void AddTexture(int atlasIndex)
    {
        float y = atlasIndex / BlockData.TextureAtlasBlockWidth;
        float x = atlasIndex - y * BlockData.TextureAtlasBlockWidth;
        float pad = BlockData.PaddingWidth;

        x *= BlockData.NormalizedTextureWidth;
        y *= BlockData.NormalizedTextureWidth;
        y = 1f - y - BlockData.NormalizedTextureWidth;

        uvs.Add(new Vector2(x + pad, y + pad));
        uvs.Add(new Vector2(x + pad, y - pad + BlockData.NormalizedTextureWidth));
        uvs.Add(new Vector2(x - pad + BlockData.NormalizedTextureWidth, y + pad));
        uvs.Add(new Vector2(x - pad + BlockData.NormalizedTextureWidth, y - pad + BlockData.NormalizedTextureWidth));
    }

    #endregion Drawing
}