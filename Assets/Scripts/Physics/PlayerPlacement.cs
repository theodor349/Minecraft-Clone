using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacement : MonoBehaviour
{
    [SerializeField] private Transform camPos;
    [SerializeField] private float range;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform crackedBlock;
    [SerializeField] private Material[] crackedMaterials;

    private int layermask = 1 << 8;
    private World world;

    private MeshRenderer crackedMesh;
    private Vector3Int blockToPunch;

    private float hardness;
    private float destroyProgress;

    private void Start()
    {
        crackedMesh = crackedBlock.GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        world = World.Instance;

        if (Input.GetMouseButton(0))
            PunchBlock();
        if (Input.GetMouseButtonUp(0))
            ResetPunchBlock();
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int pos = BlockPlacement();
            if (inventory.GetSlectedItem() != null)
                world.EditBlock(pos, inventory.GetSlectedItem().BlockType);
        }
    }

    private void PunchBlock()
    {
        Vector3Int block = BlockDestroy();
        if (block == new Vector3Int(-1, -1, -1))
            return;

        if (block != blockToPunch)
        {
            ResetPunchBlock();

            blockToPunch = block;
            crackedBlock.position = blockToPunch + new Vector3(.5f, .5f, .5f);
            hardness = world.BlockTypes[world.GetBlockTypeAt(blockToPunch)].Hardness;
        }

        destroyProgress += Time.deltaTime / hardness;

        if(destroyProgress >= 1)
        {
            world.EditBlock(blockToPunch, 0);
            ResetPunchBlock();
        }

        crackedMesh.material = crackedMaterials[(int)(destroyProgress * crackedMaterials.Length)];
    }

    private void ResetPunchBlock()
    {
        destroyProgress = 0.0f;
        blockToPunch = new Vector3Int(-1, -1, -1);
        crackedBlock.position = new Vector3Int(-1, -1, -1);
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
