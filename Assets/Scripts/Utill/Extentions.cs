using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    
    public static Vector3Int Floor(this Vector3 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }
}
