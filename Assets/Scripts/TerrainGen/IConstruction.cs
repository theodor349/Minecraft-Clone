using UnityEngine;

public interface IConstruction
{
    void Construct();
}

public class StructureConstruction : IConstruction
{
    private World world;
    private Vector3Int pos;
    private Structure structure;

    public StructureConstruction(World world, Vector3Int pos, Structure structure)
    {
        this.world = world;
        this.pos = pos;
        this.structure = structure;
    }

    public void Construct()
    {
        Structures.BuildStructure(world, pos, structure);
    }
}
