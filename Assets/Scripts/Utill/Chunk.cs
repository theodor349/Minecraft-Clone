using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum Direction { Center, North, South, West, East };
public class Chunk
{
    GameObject obj;
    Vector3Int chunkPos;
    Vector3Int chunkCoord;
    MeshRenderer rendere;
    MeshFilter filter;
    MeshCollider collider;

    World world;

    Material mat;
    Mesh mesh;
    SpriteAtlas blockAtlas;
    List<Vector3> verts = new List<Vector3>();
    List<int> tries = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] blocks;

    int vertexIndex = 0;

    public Chunk(World world, Vector3Int pos, bool drawOnInit = true)
    {
        this.world = world;
        mat = world.Material;
        chunkCoord = pos;
        chunkPos = new Vector3Int(pos.x * BlockData.ChunkWidth, pos.y, pos.z * BlockData.ChunkWidth);

        obj = new GameObject();
        obj.transform.position = chunkPos;
        obj.transform.SetParent(world.transform);
        obj.name = "Chunk " + chunkCoord.x + "_" + chunkCoord.z;

        rendere = obj.AddComponent<MeshRenderer>();
        filter = obj.AddComponent<MeshFilter>();
        collider = obj.AddComponent<MeshCollider>();

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
                    blocks[x, y, z] = world.WorldGenGetBlockType(new Vector3Int(x + chunkPos.x, y, z + chunkPos.z));
                }
            }
        }
    }

    public byte GetBlockTypeAt(Vector3Int pos)
    {
        return blocks[pos.x, pos.y, pos.z];
    }

    public void EditBlock(Vector3Int pos, byte type)
    {
        blocks[pos.x, pos.y, pos.z] = type;
        Update();

        var edge = IsOnEdgeOfChunk(pos);
        if(edge == Direction.North)
            world.UpdateChunk(chunkCoord + new Vector3Int(0, 0, 1));
        else if(edge == Direction.South)
            world.UpdateChunk(chunkCoord + new Vector3Int(0, 0, -1));
        else if (edge == Direction.West)
            world.UpdateChunk(chunkCoord + new Vector3Int(-1, 0, 0));
        else if (edge == Direction.East)
            world.UpdateChunk(chunkCoord + new Vector3Int(1, 0, 0));
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
        float x = atlasIndex - y * BlockData.TextureAtlasBlockWidth;

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
