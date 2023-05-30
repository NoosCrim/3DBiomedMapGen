using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public readonly Vector2 Pos;
    public readonly int InChunkID;
    public readonly Biome Biome;
    public readonly Chunk Chunk;
    public readonly Map Map;
    public Tile(Vector2 pos, Biome biome, Chunk chunk, Map map, int inChunkID)
    {
        this.Pos = pos;
        this.InChunkID = inChunkID;
        this.Biome = biome;
        this.Chunk = chunk;
        this.Map = map;
    }
    public Vector3 RandPosInTile(Map map)
    {
        return new Vector3(Mathf.Clamp01((float)Map.Random.NextDouble()) * map.TileSize + Pos.x, 0, Mathf.Clamp01((float)Map.Random.NextDouble()) * map.TileSize + Pos.y);
    }
}
