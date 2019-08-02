using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacement : MonoBehaviour
{
    [SerializeField] private Transform camPos;
    [SerializeField] private float range;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform crakedBlock;
    [SerializeField] private Material[] crakedMaterials;

    private int layermask = 1 << 8;
    private World world;

    private Vector3Int blockToPunch;
    private float startTimeToDestroy;
    private float timeToDestroy;

    private void Update()
    {
        world = World.Instance;

        if (Input.GetMouseButton(0))
            PunchBlock();
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int pos = BlockPlacement();
            if (inventory.GetSlectedItem() != null)
                world.EditBlock(pos , inventory.GetSlectedItem().BlockType);
        }
    }

    private void PunchBlock()
    {
        Vector3Int block = BlockDestroy();
        if (block != blockToPunch)
        {
            if (block == new Vector3Int(-1, -1, -1))
                return;

            blockToPunch = block;
            timeToDestroy = world.BlockTypes[world.GetBlockTypeAt(blockToPunch)].Hardness;
        }

        timeToDestroy -= Time.deltaTime;


        if(timeToDestroy <= 0)
        {
            world.EditBlock(blockToPunch, 0);
            blockToPunch = new Vector3Int(-1, -1, -1);
        }
    }

    public Vector3Int BlockDestroy()
    {
        RaycastHit hit;
        if(Physics.Raycast(camPos.position, camPos.forward, out hit, range, layermask))
        {
            return (hit.point - hit.normal * 0.5f).Floor();
        }

        return new Vector3Int(-1, -1, -1);
    }

    public Vector3Int BlockPlacement()
    {
        RaycastHit hit;
        if (Physics.Raycast(camPos.position, camPos.forward, out hit, range, layermask))
        {
            return (hit.point + hit.normal * 0.5f).Floor();
        }

        return new Vector3Int(-1, -1, -1);
    }

}
