using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Chunk
{
    GameObject obj;
    Vector3Int chunkPos;
    Vector3Int chunkCoord;
    MeshRenderer rendere;
    MeshFilter filter;

    World world;

    Material mat;
    Mesh mesh;
    SpriteAtlas blockAtlas;
    List<Vector3> verts = new List<Vector3>();
    List<int> tries = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxels;

    int vertexIndex = 0;

    public Chunk(World world, Vector3Int pos, bool drawOnInit = true)
    {
        this.world = world;
        mat = world.Material;
        chunkCoord = pos;
        chunkPos = new Vector3Int(pos.x * VoxelData.ChunkWidth, pos.y, pos.z * VoxelData.ChunkWidth);

        obj = new GameObject();
        obj.transform.position = chunkPos;
        obj.transform.SetParent(world.transform);
        obj.name = "Chunk " + chunkCoord.x + "_" + chunkCoord.z;

        rendere = obj.AddComponent<MeshRenderer>();
        filter = obj.AddComponent<MeshFilter>();

        GenerateChunk();
        if (drawOnInit)
            DrawChunk();
    }

    public bool IsVoxelSolid(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= VoxelData.ChunkWidth || pos.z < 0 || pos.z >= VoxelData.ChunkWidth)
            return world.IsVoxelSolid(pos + chunkPos);
        if (pos.y < 0)
            return true;
        if (pos.y >= VoxelData.ChunkHeight)
            return false;

        return (voxels[pos.x, pos.y, pos.z] != 0);
    }

    private void GenerateChunk()
    {
        voxels = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    voxels[x, y, z] = world.GetBlockType(new Vector3Int(x + chunkPos.x, y, z + chunkPos.z));
                }
            }
        }
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

        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int y = 0; y < VoxelData.ChunkHeight; y++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
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
            AddTexture(world.VoxelTypes[voxels[pos.x, pos.y, pos.z]].GetFaceTexture((Face)face));

            vertexIndex += 4;
        }
    }

    private bool ShouldDrawVoxel(Vector3Int pos, int face)
    {
        return IsVoxelSolid(pos + VoxelData.Neighbors[face]);
    }

    private void AddVerticies(Vector3Int pos, int face)
    {
        verts.Add(pos + VoxelData.Vertices[VoxelData.Triangles[face, 0]]);
        verts.Add(pos + VoxelData.Vertices[VoxelData.Triangles[face, 1]]);
        verts.Add(pos + VoxelData.Vertices[VoxelData.Triangles[face, 2]]);
        verts.Add(pos + VoxelData.Vertices[VoxelData.Triangles[face, 3]]);
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
        float y = atlasIndex / VoxelData.TextureAtlasBlockWidth;
        float x = atlasIndex - y * VoxelData.TextureAtlasBlockWidth;

        x *= VoxelData.NormalizedTextureWidth;
        y *= VoxelData.NormalizedTextureWidth;
        y = 1f - y - VoxelData.NormalizedTextureWidth;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedTextureWidth));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTextureWidth, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedTextureWidth, y + VoxelData.NormalizedTextureWidth));
    }
    #endregion

}
