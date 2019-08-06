using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameobject : MonoBehaviour
{
    private Item item;
    private World world;

    private MeshRenderer rendere;
    private MeshFilter filter;
    private Material mat;
    private Mesh mesh;
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tries = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private int vertexIndex = 0;

    public void Initialize(Vector3 pos, Item item)
    {
        world = World.Instance;
        this.item = item;

        transform.position = pos + new Vector3(0.5f, 0.5f, 0.5f);
        transform.parent = transform;
        name = "Item";
        tag = "Item";
        gameObject.layer = 9;

        mat = world.Mat;
        gameObject.AddComponent<CollisionObject>();
        gameObject.AddComponent<Rigidbody>();

        var col = gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(0.25f, 0.25f, 0.25f);
        col.material = world.PhysicsMat;

        var scol = gameObject.AddComponent<SphereCollider>();
        scol.radius = 0.5f;
        scol.isTrigger = true;

        rendere = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();

        DrawVoxel();
    }

    public Item GetItem()
    {
        return item;
    }

    public void Take(int amount)
    {
        item.StackSize -= amount;
        if (item.StackSize == 0)
            Destroy(gameObject);
    }

    private void DrawVoxel()
    {
        for (int face = 0; face < 6; face++)
        {
            AddVerticies(face);
            AddTriangles();
            AddTexture(world.BlockTypes[item.BlockType].GetFaceTexture((Face)face));

            vertexIndex += 4;
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(tries.ToArray(), 0);
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        filter.mesh = mesh;
        rendere.material = mat;
    }

    private void AddVerticies(int face)
    {
        verts.Add((BlockData.Vertices[BlockData.Triangles[face, 0]] + new Vector3(-0.5f, -0.5f, -0.5f)) * 0.25f);
        verts.Add((BlockData.Vertices[BlockData.Triangles[face, 1]] + new Vector3(-0.5f, -0.5f, -0.5f)) * 0.25f);
        verts.Add((BlockData.Vertices[BlockData.Triangles[face, 2]] + new Vector3(-0.5f, -0.5f, -0.5f)) * 0.25f);
        verts.Add((BlockData.Vertices[BlockData.Triangles[face, 3]] + new Vector3(-0.5f, -0.5f, -0.5f)) * 0.25f);
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
}
