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
            new Vector3(width, 0, width),
            new Vector3(width, 0, -width),
            new Vector3(-width, 0, width),
            new Vector3(-width, 0, -width),

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

    private void Update()
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
        Debug.Log("HI");
        dist.y = 0;

        bool hit = false;

        for (int i = 1; i < points.Length; i += 2)
        {
            hit = hit || world.IsVoxelSolid((transform.position + points[i] + dist).Floor());
        }

        return hit;
    }

    private void OnDrawGizmos()
    {
        if (points == null)
            return;

        float width = Width * 0.5f;

        for (int i = 0; i < points.Length; i++)
        {
            if (i % 2 == 0)
                Gizmos.color = Color.white;
            else
                Gizmos.color = Color.red;

            Vector3 point = points[i];
            Gizmos.DrawSphere(transform.position + point, 0.1f);
        }
    }
}
