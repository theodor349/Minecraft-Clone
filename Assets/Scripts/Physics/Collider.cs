using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public float Width = 0.8f;
    public float Height = 1.8f;

    private World world;
    private float width;

    private void Start()
    {
        world = World.Instance;
        width = Width / 2f;
    }

    public bool CheckY(float distance)
    {
        if (distance > 0)
        {
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, Height + distance, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, Height + distance, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, Height + distance, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, Height + distance, -width)).Floor())) return true;
        }
        else
        {
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, distance, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, distance, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, distance, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, distance, -width)).Floor())) return true;
        }
        return false;
    }

    public bool CheckX(float distance)
    {
        if (distance > 0)
        {
            if(world.IsVoxelSolid((transform.position + new Vector3(width + distance, Height, -width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(width + distance, Height, width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(width + distance, 0, width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(width + distance, 0, -width)).Floor())) return true;

        }
        else
        {
            if (world.IsVoxelSolid((transform.position + new Vector3(-width + distance, Height, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width + distance, Height, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width + distance, 0, width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width + distance, 0, -width)).Floor())) return true;
        }

        return false;
    }

    public bool CheckZ(float distance)
    {
        if (distance > 0)
        {
            if(world.IsVoxelSolid((transform.position + new Vector3(-width, Height, width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(width, Height, width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(-width, 0, width)).Floor())) return true;
            if(world.IsVoxelSolid((transform.position + new Vector3(width, 0, width)).Floor())) return true;
        }
        else
        {
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, Height, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, Height, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(-width, 0, -width)).Floor())) return true;
            if (world.IsVoxelSolid((transform.position + new Vector3(width, 0, -width)).Floor())) return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        float w = Width * 0.5f;
        float h = Height * 0.5f;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, h, 0), new Vector3(Width, Height, Width));
    }
}
