using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public float Width = 0.8f;
    public float Height = 1.8f;

    private World world;

    private void Start()
    {
        world = World.Instance;
    }

    public bool CheckY(float distance)
    {
        if (distance > 0)
            return world.IsVoxelSolid((transform.position + new Vector3(0, Height + distance, 0)).Floor());
        else
            return world.IsVoxelSolid((transform.position + new Vector3(0, distance, 0)).Floor());
    }

    public bool CheckX(float distance)
    {
        if (distance > 0)
            return world.IsVoxelSolid((transform.position + new Vector3(Width * 0.5f + distance, 0, 0)).Floor());
        else
            return world.IsVoxelSolid((transform.position + new Vector3(-Width * 0.5f - distance, 0, 0)).Floor());
    }

    public bool CheckZ(float distance)
    {
        if (distance > 0)
            return world.IsVoxelSolid((transform.position + new Vector3(0, 0, Width * 0.5f + distance)).Floor());
        else
            return world.IsVoxelSolid((transform.position + new Vector3(0, 0, -Width * 0.5f - distance)).Floor());
    }

}
