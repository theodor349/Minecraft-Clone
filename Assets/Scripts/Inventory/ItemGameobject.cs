using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameobject : MonoBehaviour
{
    private Item item;
    private World world;

    public void Initialize(Vector3 pos, Item item)
    {
        world = World.Instance;
        this.item = item;

        // Setup
        SetupGameObj(pos);
        AddComponents();
        AddColliders();

        // Drawing
        DrawCube();
    }

    private void SetupGameObj(Vector3 pos)
    {
        transform.position = pos + new Vector3(0.5f, 0.5f, 0.5f);
        transform.parent = world.ItemParent;

        gameObject.name = "Item";
        gameObject.tag = "Item";
        gameObject.layer = 9;
    }

    private void AddComponents()
    {
        gameObject.AddComponent<CollisionObject>();
        gameObject.AddComponent<Rigidbody>();
    }

    private void AddColliders()
    {
        var col = gameObject.AddComponent<BoxCollider>();
        col.size = new Vector3(ItemObjData.Radius * 2f, ItemObjData.Radius * 2f, ItemObjData.Radius * 2f);
        col.material = world.PhysicsMat;

        var scol = gameObject.AddComponent<SphereCollider>();
        scol.radius = 0.75f;
        scol.isTrigger = true;
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

    private void DrawCube()
    {
        var mesh = new Mesh();
        mesh.vertices = ItemObjData.Verts;

        var uvs = new List<Vector2>();
        for (int face = 0; face < 6; face++)
        {
            AddTexture(world.GetBlock(item.BlockType).GetFaceTexture((Face)face), ref uvs);
        }

        mesh.SetTriangles(ItemObjData.Tries, 0);
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = world.Mat;
    }

    private void AddTexture(int atlasIndex, ref List<Vector2> uvs)
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
}

class ItemObjData
{
    public static float Radius = 0.125f;

    public static Vector3[] Verts = new Vector3[]
    {
            // Back face
            new Vector3(-Radius, -Radius, -Radius),
            new Vector3(-Radius, Radius, -Radius),
            new Vector3(Radius, -Radius, -Radius),
            new Vector3(Radius, Radius, -Radius),
            // Front face
            new Vector3(Radius, -Radius, Radius),
            new Vector3(Radius, Radius, Radius),
            new Vector3(-Radius, -Radius, Radius),
            new Vector3(-Radius, Radius, Radius),
            // Top face
            new Vector3(-Radius, Radius, -Radius),
            new Vector3(-Radius, Radius, Radius),
            new Vector3(Radius, Radius, -Radius),
            new Vector3(Radius, Radius, Radius),
            // Buttom face
            new Vector3(Radius, -Radius, -Radius),
            new Vector3(Radius, -Radius, Radius),
            new Vector3(-Radius, -Radius, -Radius),
            new Vector3(-Radius, -Radius, Radius),
            // Left face
            new Vector3(-Radius, -Radius, Radius),
            new Vector3(-Radius, Radius, Radius),
            new Vector3(-Radius, -Radius, -Radius),
            new Vector3(-Radius, Radius, -Radius),
            // Right face
            new Vector3(Radius, -Radius, -Radius),
            new Vector3(Radius, Radius, -Radius),
            new Vector3(Radius, -Radius, Radius),
            new Vector3(Radius, Radius, Radius),
    };

    public static int[] Tries = new int[]
    {
          0, 1, 2, 2, 1, 3,
          4, 5, 6, 6, 5, 7,
          8, 9,10,10, 9,11,
         12,13,14,14,13,15,
         16,17,18,18,17,19,
         20,21,22,22,21,23
    };
}