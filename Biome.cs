using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public struct BiomeIDSize
{
    public int ID;
    public float Size;
}
[System.Serializable]
public class BiomeType
{
    public string Name;
    public int TexAtlasIndex;
    [SerializeField]
    public List<StructureIDProbability> Structures;
}
[System.Serializable]
public class Biome
{
    public const float OVERLAP_TOLERANCE = 0.0f;
    public readonly BiomeType BiomeType;
    public readonly Dictionary<Vector2, Tile> Tiles;
    public Vector2 pos;
    public Vector2 PosVector
    {
        get
        {
            return new Vector2(pos.x, pos.y);
        }
        private set 
        {
            
        }
    }
    public readonly float Size;
    public readonly List<Biome> Neighbours;
    public Biome(BiomeType type, float x, float y, float size)
    {
        this.BiomeType = type;
        this.pos.x = x;
        this.pos.y = y;
        this.Size = size;
        this.Neighbours = new List<Biome>();
        this.Tiles = new Dictionary<Vector2, Tile>();
    }
}