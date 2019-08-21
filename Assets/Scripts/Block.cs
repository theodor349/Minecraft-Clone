using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockType { Air, Bedrock, Dirt, Grass, Stone, Cobblestone, Planks, Log, Glass, Leaves, Furnace }
public enum Direction { Nothing, North, South, West, East };
public enum Face { Back, Front, Top, Button, Left, Right };

[CreateAssetMenu(fileName ="Voxels", menuName = "Minecraft/Voxels")]
public class Block : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public bool IsSolid;
    public bool IsTransparent;
    public bool IsRotationSpecific; // Can only rotate around the up-axis
    public float Hardness;
    public byte MiningLevel;
    public string BreaksInto;

    [Header("Face Textures")]
    public int Back;
    public int Front;
    public int Top;
    public int Buttom;
    public int Left;
    public int Right;

    public int GetFaceTexture(Face face, Direction rotation)
    {
        if (!IsRotationSpecific || rotation == Direction.Nothing)
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

        switch (rotation)
        {
            case Direction.South:
                switch (face)
                {
                    case Face.Back:
                        return Front;
                    case Face.Front:
                        return Back;
                    case Face.Top:
                        return Top;
                    case Face.Button:
                        return Buttom;
                    case Face.Left:
                        return Right;
                    case Face.Right:
                        return Left;
                    default:
                        Debug.LogError("GetFaceTexture(): " + face.ToString() + " is not indexed ");
                        return Left;
                }
            case Direction.North:
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
            case Direction.West:
                switch (face)
                {
                    case Face.Back:
                        return Right;
                    case Face.Front:
                        return Left;
                    case Face.Top:
                        return Top;
                    case Face.Button:
                        return Buttom;
                    case Face.Left:
                        return Back;
                    case Face.Right:
                        return Front;
                    default:
                        Debug.LogError("GetFaceTexture(): " + face.ToString() + " is not indexed ");
                        return Left;
                }
            case Direction.East:
                switch (face)
                {
                    case Face.Back:
                        return Left;
                    case Face.Front:
                        return Right;
                    case Face.Top:
                        return Top;
                    case Face.Button:
                        return Buttom;
                    case Face.Left:
                        return Front;
                    case Face.Right:
                        return Back;
                    default:
                        Debug.LogError("GetFaceTexture(): " + face.ToString() + " is not indexed ");
                        return Left;
                }
            default:
                Debug.LogError("GetFaceTexture(): " + rotation.ToString() + " is not indexed ");
                return Left;
        }
    }

    public BlockType Break(byte miningLevel)
    {
        if (BreaksInto.Equals("Nothing") || miningLevel < MiningLevel)
            return BlockType.Air;
        else if (BreaksInto.Length == 0)
        {
            Enum.TryParse(Name, out BlockType type);
            return type;
        }
        else
        {
            Enum.TryParse(BreaksInto, out BlockType type);
            return type;
        }
    }

}
