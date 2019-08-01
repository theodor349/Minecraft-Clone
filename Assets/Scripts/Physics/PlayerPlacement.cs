using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlacement : MonoBehaviour
{
    //[SerializeField] private Transform blockDestroy;
    //[SerializeField] private Transform blockPlace;
    [SerializeField] private Transform camPos;
    [SerializeField] private float range;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Collider collider;
    private int layermask = 1 << 8;

    private World world;
    private Vector3Int blockDestroyPos;
    private Vector3Int blockPlacementPos;

    private void Update()
    {
        world = World.Instance;

        if (Input.GetMouseButtonDown(0))
            world.EditBlock(BlockDestroy(), 0);
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int pos = BlockPlacement();
            if (inventory.GetSlectedItem() != null)
                world.EditBlock(pos , inventory.GetSlectedItem().BlockType);
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
