using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum Direction { Center, North, South, West, East };
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

    private Material mat;
    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tries = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private byte[,,] blocks;

    // Drawing
    private int vertexIndex = 0;

    public Chunk(World world, Vector2Int pos, bool drawOnInit = true)
    {
        this.world = world;
        mat = world.Mat;
        chunkCoord = pos;
        chunkPos = new Vector3Int(pos.x * BlockData.ChunkWidth, 0, pos.y * BlockData.ChunkWidth);

        obj = new GameObject();
        obj.layer = 8; 
        obj.transform.position = chunkPos;
        obj.transform.SetParent(world.transform);
        obj.name = "Chunk " + chunkCoord.x + "_" + chunkCoord.y;

        rendere = obj.AddComponent<MeshRenderer>();
        filter = obj.AddComponent<MeshFilter>();
        collider = obj.AddComponent<MeshCollider>();
        collider.enabled = false;

        GenerateChunk();
        if (drawOnInit)
            DrawChunk();
    }

    public bool IsVoxelSolid(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= BlockData.ChunkWidth || pos.z < 0 || pos.z >= BlockData.ChunkWidth)
            return world.IsVoxelSolid(pos + chunkPos);
        if (pos.y < 0)
            return true;
        if (pos.y >= BlockData.ChunkHeight)
            return false;

        return (blocks[pos.x, pos.y, pos.z] != 0);
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
                    blocks[x, y, z] = (byte)world.WorldGenGetBlockType(new Vector3Int(x + chunkPos.x, y, z + chunkPos.z));
                }
            }
        }
    }

    public byte GetBlockTypeAt(Vector3Int pos)
    {
        return blocks[pos.x, pos.y, pos.z];
    }

    public void EditBlock(Vector3Int pos, BlockType type)
    {
        blocks[pos.x, pos.y, pos.z] = (byte)type;
        Update();

        var edge = IsOnEdgeOfChunk(pos);
        if(edge == Direction.North)
            world.UpdateChunk(chunkCoord + new Vector2Int(0, 1));
        else if(edge == Direction.South)
            world.UpdateChunk(chunkCoord + new Vector2Int(0, -1));
        else if (edge == Direction.West)
            world.UpdateChunk(chunkCoord + new Vector2Int(-1, 0));
        else if (edge == Direction.East)
            world.UpdateChunk(chunkCoord + new Vector2Int(1, 0));
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

        return Direction.Center;
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
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        filter.mesh = mesh;
        if (collisionPointers > 0)
            collider.sharedMesh = mesh;
        rendere.material = mat;
        collider.sharedMesh = mesh;
    }

    private void ClearMesh()
    {
        vertexIndex = 0;
        verts.Clear();
        tries.Clear();
        uvs.Clear();
    }

    private void DrawVoxel(Vector3Int pos)
    {
        if (!IsVoxelSolid(pos))
            return;

        for (int face = 0; face < 6; face++)
        {
            if (ShouldDrawVoxel(pos, face))
                continue;

            AddVerticies(pos, face);
            AddTriangles();
            AddTexture(world.BlockTypes[blocks[pos.x, pos.y, pos.z]].GetFaceTexture((Face)face));

            vertexIndex += 4;
        }
    }

    private bool ShouldDrawVoxel(Vector3Int pos, int face)
    {
        return IsVoxelSolid(pos + BlockData.Neighbors[face]);
    }

    private void AddVerticies(Vector3Int pos, int face)
    {
        verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 0]]);
        verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 1]]);
        verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 2]]);
        verts.Add(pos + BlockData.Vertices[BlockData.Triangles[face, 3]]);
    }

    private void AddTriangles()
    {
        tries.Add(vertexIndex + 0);
        tries.Add(vertexIndex + 1);
        tries.Add(vertexIndex + 2);
        tries.Add(vertexIndex + 2);
        tries.Add(vertexIndex + 1);
        tries.Add(vertexIndex + 3);
    }

    private void AddTexture(int atlasIndex)
    {
        float y = atlasIndex / BlockData.TextureAtlasBlockWidth;
        float x = atlasIndex - (y * BlockData.TextureAtlasBlockWidth);

        x *= BlockData.NormalizedTextureWidth;
        y *= BlockData.NormalizedTextureWidth;
        y = 1f - y - BlockData.NormalizedTextureWidth;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + BlockData.NormalizedTextureWidth));
        uvs.Add(new Vector2(x + BlockData.NormalizedTextureWidth, y));
        uvs.Add(new Vector2(x + BlockData.NormalizedTextureWidth, y + BlockData.NormalizedTextureWidth));
    }
    #endregion

}
