using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingBlock : MonoBehaviour
{
    public Block Block;
    public BlockType Type;
    public Direction Rotation;

    private Rigidbody rb;
    private World world;

    private void Start()
    {
        Debug.Log("Sand");
        world = World.Instance;
        Draw();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Player"))
        {
            Debug.Log("Player Hit");
        }
        else if (collision.collider.tag.Equals("Chunk"))
        {
            World.Instance.EditBlock(transform.position.Floor(), Type, Rotation);
            Destroy(gameObject);
        }
    }

    private void Draw()
    {
        var rendere = gameObject.GetComponent<MeshRenderer>();
        var filter = gameObject.GetComponent<MeshFilter>();
        var mesh = filter.mesh;
        var uvs = new List<Vector2>();

        rendere.material = Block.IsTransparent ? world.TransparentMat : world.Mat;

        for (int face = 0; face < 6; face++)
        {
            AddTexture(Block.GetFaceTexture((Face)face, Direction.Nothing), ref uvs);
        }

        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        filter.mesh = mesh;
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
