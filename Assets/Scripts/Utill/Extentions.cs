using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    
    public static Vector3Int Floor(this Vector3 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }

    public static Vector2Int Floor(this Vector2 v)
    {
        return new Vector2Int((int)v.x, (int)v.y);
    }

    public static Vector3 RotateAroundY(this Vector3 v, float angle)
    {
        Vector3 result = new Vector3();
        Vector3 eulerAngles = new Vector3(0, angle * ((float)Math.PI / 180));
        result.x = v.x * (float)Math.Cos(eulerAngles.y) + v.z * (float)Math.Sin(eulerAngles.y);
        result.z = -v.x * (float)Math.Sin(eulerAngles.y) + v.z * (float)Math.Cos(eulerAngles.y);
        result.y = v.y;
        return result;
    }
}
