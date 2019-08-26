using System.Collections.Generic;
using UnityEngine;

public class CollisionObject : MonoBehaviour
{
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private int range = 7;

    private World world;
    private Vector3 lastPos = new Vector3(-1, -1, -1);
    private List<Vector2Int> lastChunks = new List<Vector2Int>();

    private void Start()
    {
        world = World.Instance;
    }

    private void FixedUpdate()
    {
        if (lastPos.x == transform.position.x && lastPos.z == transform.position.z)
            return;
        lastPos = transform.position;

        List<Vector2Int> chunks = new List<Vector2Int>();
        chunks.Add(World.GetChunkCoord(transform.position.Floor()));
        Vector3Int pos = World.GetBlockPosInChunk(transform.position.Floor());

        if (isPlayer)
            PlayerObject(ref chunks, pos);
        else
            SimpleObject(ref chunks, pos);

        foreach (var chunk in chunks)
        {
            if (!lastChunks.Contains(chunk))
                world.AddChunksToCollision(chunk);
        }

        lastChunks.RemoveAll(item => chunks.Contains(item));
        foreach (var chunk in lastChunks)
        {
            world.RemoveChunksToCollision(chunk);
        }

        lastChunks = chunks;
    }

    private void SimpleObject(ref List<Vector2Int> chunks, Vector3Int pos)
    {
        if (pos.x == 0)
            chunks.Add(chunks[0] + new Vector2Int(-1, 0));
        if (pos.x == BlockData.ChunkWidth - 1)
            chunks.Add(chunks[0] + new Vector2Int(1, 0));

        if (pos.z == 0)
            chunks.Add(chunks[0] + new Vector2Int(0, -1));
        if (pos.z == BlockData.ChunkWidth - 1)
            chunks.Add(chunks[0] + new Vector2Int(0, 1));

        if (pos.x == 0 && pos.z == 0)
            chunks.Add(chunks[0] + new Vector2Int(-1, -1));
        else if (pos.x == BlockData.ChunkWidth - 1 && pos.z == BlockData.ChunkWidth - 1)
            chunks.Add(chunks[0] + new Vector2Int(1, 1));
        else if (pos.x == 0 && pos.z == BlockData.ChunkWidth - 1)
            chunks.Add(chunks[0] + new Vector2Int(-1, 1));
        else if (pos.x == BlockData.ChunkWidth - 1 && pos.z == 0)
            chunks.Add(chunks[0] + new Vector2Int(1, -1));
    }

    private void PlayerObject(ref List<Vector2Int> chunks, Vector3Int pos)
    {
        if (pos.x <= range)
            chunks.Add(chunks[0] + new Vector2Int(-1, 0));
        if (pos.x >= BlockData.ChunkWidth - 1 - range)
            chunks.Add(chunks[0] + new Vector2Int(1, 0));

        if (pos.z <= range)
            chunks.Add(chunks[0] + new Vector2Int(0, -1));
        if (pos.z >= BlockData.ChunkWidth - range)
            chunks.Add(chunks[0] + new Vector2Int(0, 1));

        if (pos.x <= range && pos.z <= range)
            chunks.Add(chunks[0] + new Vector2Int(-1, -1));
        else if (pos.x >= BlockData.ChunkWidth - 1 - range && pos.z >= BlockData.ChunkWidth - 1 - range)
            chunks.Add(chunks[0] + new Vector2Int(1, 1));
        else if (pos.x <= range && pos.z >= BlockData.ChunkWidth - 1 - range)
            chunks.Add(chunks[0] + new Vector2Int(-1, 1));
        else if (pos.x >= BlockData.ChunkWidth - 1 - range && pos.z <= range)
            chunks.Add(chunks[0] + new Vector2Int(1, -1));
    }
}