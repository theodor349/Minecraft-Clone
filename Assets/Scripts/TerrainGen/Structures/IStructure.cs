using System;
using UnityEngine;

public enum Structure { Tree }

public interface IStructure
{
    void Build(World world, Vector3Int GroundPos);
}

public static class Structures
{
    private static IStructure[] structures;

    public static void Init()
    {
        OrganizeStructures();
    }

    private static void OrganizeStructures()
    {
        structures = new IStructure[Enum.GetValues(typeof(Structure)).Length];

        for (int i = 0; i < structures.Length; i++)
        {
            Type type = Type.GetType(Enum.GetName(typeof(Structure), i));

            if (type == null)
                Debug.LogError("Structures: A structures have been added to the enum and no class created");

            structures[i] = (IStructure)Activator.CreateInstance(type);
        }
    }

    public static void BuildStructure(World world, Vector3Int startPos, Structure structure)
    {
        structures[(int)structure].Build(world, startPos);
    }
}