using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextureAtlas
{
    public int Size;
    public float PaddingSize = 0.125f;
    public Material MapMaterial;
    public float TileStep
    {
        get { return 1.0f / Size; }
    }
    public float Padding
    {
        get { return PaddingSize/Size; }
    }
    public float TileSize
    {
        get { return (1.0f - PaddingSize * 2) / Size; }
    }
}
