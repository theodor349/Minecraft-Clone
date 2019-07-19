using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider : MonoBehaviour
{
    public float Width = 0.8f;
    public float Height = 1.8f;

    private World world;
    private float width;
    private float height;

    private Vector3[] points;
    private Vector3[] initialPoints;

    private void Start()
    {
        world = World.Instance;
        width = Width * 0.5f;
        height = Height * 0.5f;

        initialPoints = new Vector3[]
        {
            new Vector3(-width, 0, width),
            new Vector3(-width, 0, -width),
            new Vector3(width, 0, width),
            new Vector3(width, 0, -width),

            new Vector3(-width, height, width),
            new Vector3(-width, height, -width),
            new Vector3(width, height, width),
            new Vector3(width, height, -width),

            new Vector3(-width, Height, width),
            new Vector3(-width, Height, -width),
            new Vector3(width, Height, width),
            new Vector3(width, Height, -width),
        };
        points = new Vector3[initialPoints.Length];
    }

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3 rotaion = transform.rotation.eulerAngles;

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = initialPoints[i].RotateAroundY(rotaion.y);
        }
    }

    public bool Down(float dist)
    {
        bool hit = false;
        Vector3 down = new Vector3(0, dist, 0);
        for (int i = 0; i < 4; i++)
        {
            hit = hit || world.IsVoxelSolid((transform.position + points[i] + down).Floor());
        }

        return hit;
    }

    public bool Up(float dist)
    {
        Vector3 up = new Vector3(0, dist, 0);
        bool hit = false;

        for (int i = points.Length - 4; i < points.Length; i++)
        {
            hit = hit || world.IsVoxelSolid((transform.position + points[i] + up).Floor());
        }

        return hit;
    }

    public bool Forward(Vector3 dist)
    {
        dist.y = 0;
        bool hit = false;

        for (int i = 0; i < points.Length; i += 2)
        {
            hit = hit || world.IsVoxelSolid((transform.position + points[i] + dist).Floor());
        }

        return hit;
    }

    public bool Backward(Vector3 dist)
    {
        dist.y = 0;
        bool hit = false;

        for (int i = 1; i < points.Length; i += 2)
        {
            hit = hit || world.IsVoxelSolid((transform.position + points[i] + dist).Floor());
        }

        return hit;
    }

    public bool Left(Vector3 dist)
    {
        dist.y = 0;
        bool hit = false;

        hit = hit || world.IsVoxelSolid((transform.position + points[0] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[1] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[4] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[5] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[8] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[9] + dist).Floor());

        return hit;
    }

    public bool Right(Vector3 dist)
    {
        dist.y = 0;
        bool hit = false;

        hit = hit || world.IsVoxelSolid((transform.position + points[2] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[3] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[6] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[7] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[10] + dist).Floor());
        hit = hit || world.IsVoxelSolid((transform.position + points[11] + dist).Floor());

        return hit;
    }

    private void OnDrawGizmos()
    {
        if (points == null)
            return;

        float width = Width * 0.5f;

        // Forward - Backward
        //for (int i = 0; i < points.Length; i++)
        //{
        //    if (i % 2 == 0)
        //        Gizmos.color = Color.white;
        //    else
        //        Gizmos.color = Color.red;

        //    Vector3 point = points[i];
        //    Gizmos.DrawSphere(transform.position + point, 0.1f);
        //}

        // Left - Right
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position + points[0], 0.1f);
        Gizmos.DrawSphere(transform.position + points[1], 0.1f);
        Gizmos.DrawSphere(transform.position + points[4], 0.1f);
        Gizmos.DrawSphere(transform.position + points[5], 0.1f);
        Gizmos.DrawSphere(transform.position + points[8], 0.1f);
        Gizmos.DrawSphere(transform.position + points[9], 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + points[2], 0.1f);
        Gizmos.DrawSphere(transform.position + points[3], 0.1f);
        Gizmos.DrawSphere(transform.position + points[6], 0.1f);
        Gizmos.DrawSphere(transform.position + points[7], 0.1f);
        Gizmos.DrawSphere(transform.position + points[10], 0.1f);
        Gizmos.DrawSphere(transform.position + points[11], 0.1f);

    }
}
