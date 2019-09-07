using UnityEngine;

[RequireComponent(typeof(Toolbelt))]
public class PlayerPlacement : MonoBehaviour
{
    [SerializeField] private Transform camPos;
    [SerializeField] private float range;
    [SerializeField] private Transform crackedBlock;
    [SerializeField] private Material[] crackedMaterials;
    [SerializeField] private GameObject InventoryObj;

    private Toolbelt toolbelt;
    private int layermask = 1 << 8;
    private World world;

    private MeshRenderer crackedMesh;
    private Vector3Int blockToPunch;

    private float hardness;
    private float destroyProgress;

    private void Start()
    {
        world = World.Instance;
        crackedMesh = crackedBlock.GetComponent<MeshRenderer>();
        toolbelt = GetComponent<Toolbelt>();
    }

    private void Update()
    {
        if (InventoryObj.activeSelf)
            return;

        if (Input.GetMouseButton(0))
            PunchBlock();
        if (Input.GetMouseButtonUp(0))
            ResetPunchBlock();
        else if (Input.GetMouseButtonDown(1))
            PlaceBlock();
    }

    private void PlaceBlock()
    {
        Vector3Int pos = BlockPlacement();
        if (toolbelt.GetSlectedItem() == null)
            return;

        var item = toolbelt.GetSlectedItem();

        Direction rotation = Direction.Nothing;
        if (world.GetBlock((BlockType)item.BlockType).IsRotationSpecific)
        {
            Vector3Int dif = pos - transform.position.Floor();

            if (dif.x > 0) // East
            {
                if (dif.z > 0) // North-East
                {
                    if (Mathf.Max(Mathf.Abs(dif.z), Mathf.Abs(dif.x)) == Mathf.Abs(dif.x))
                        rotation = Direction.East;
                    else
                        rotation = Direction.South;
                }
                else // South-East
                {
                    if (Mathf.Max(Mathf.Abs(dif.z), Mathf.Abs(dif.x)) == Mathf.Abs(dif.x))
                        rotation = Direction.East;
                    else
                        rotation = Direction.North;
                }
            }
            else // West
            {
                if (dif.z > 0) // North-West
                {
                    if (Mathf.Max(Mathf.Abs(dif.z), Mathf.Abs(dif.x)) == Mathf.Abs(dif.x))
                        rotation = Direction.West;
                    else
                        rotation = Direction.South;
                }
                else // South-West
                {
                    if (Mathf.Max(Mathf.Abs(dif.z), Mathf.Abs(dif.x)) == Mathf.Abs(dif.x))
                        rotation = Direction.West;
                    else
                        rotation = Direction.North;
                }
            }
        }

        world.EditBlock(pos, item.BlockType, rotation);
        toolbelt.Take(1);
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
            hardness = world.GetBlock(world.GetBlockTypeAt(blockToPunch)).Hardness;

            if(hardness < 0)
            {
                ResetPunchBlock();
                return;
            }
        }

        destroyProgress += Time.deltaTime / hardness;

        if (destroyProgress >= 1)
        {
            world.BreakBlock(blockToPunch);
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
        if (Physics.Raycast(camPos.position, camPos.forward, out hit, range, layermask))
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