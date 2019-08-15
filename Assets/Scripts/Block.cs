using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType { Air, Bedrock, Dirt, Grass, Stone, Cobblestone, Planks, Log, Glass, Leaves }
public enum Face { Back, Front, Top, Button, Left, Right };
[CreateAssetMenu(fileName ="Voxels", menuName = "Minecraft/Voxels")]
public class Block : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public bool IsSolid;
    public bool IsTransparent;
    public int Hardness;

    [Header("Face Textures")]
    public int Back;
    public int Front;
    public int Top;
    public int Buttom;
    public int Left;
    public int Right;

    public int GetFaceTexture(Face face)
    {
        switch (face)
        {
            case Face.Back:
                return Back;
            case Face.Front:
                return Front; 
            case Face.Top:
                return Top;
            case Face.Button:
                return Buttom;
            case Face.Left:
                return Left;
            case Face.Right:
                return Right;
            default:
                Debug.LogError("GetFaceTexture(): " + face.ToString() + " is not indexed ");
                return Left;
        }
    }

}
