using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chunk
{
    public const int TILES_PER_CHUNK = 16;
    public readonly Vector2 Pos;
    public readonly List<Vector3> tempVertices;
    public readonly List<Vector2> tempUVs;
    public readonly List<int> tempTriangles;
    public readonly Mesh Mesh;
    public readonly Map Map;
    public readonly List<Tile> Tiles;
    public readonly List<Structure> Structures;
    public GameObject GO;
    public readonly int ID;
    public Chunk(Vector2 pos, Map map, int ID)
    {
        this.Map = map;
        this.Pos = pos;
        this.Mesh = new Mesh();
        this.Tiles = new List<Tile>();
        this.GO = new GameObject();
        this.GO.AddComponent<MeshFilter>().mesh = (this.Mesh = new Mesh());
        this.GO.AddComponent<MeshRenderer>().material = Map.TexAtlas.MapMaterial;
        this.tempTriangles = new List<int>();
        this.tempVertices = new List<Vector3>();
        this.tempUVs = new List<Vector2>();
        this.ID = ID;
        this.Structures = new List<Structure>();
    }
    public static Vector2 getChunkPos(Vector2 pos, Map map)
    {
        Vector2 chunkPos = pos / (TILES_PER_CHUNK * map.TileSize);
        chunkPos.x = Mathf.Floor(chunkPos.x) * TILES_PER_CHUNK * map.TileSize;
        chunkPos.y = Mathf.Floor(chunkPos.y) * TILES_PER_CHUNK * map.TileSize;
        return chunkPos;
    }
    public static Vector2 getChunkPos(Vector3 pos, Map map)
    {
        Vector3 chunkPos = pos / (TILES_PER_CHUNK * map.TileSize);
        chunkPos.x = Mathf.Floor(chunkPos.x) * TILES_PER_CHUNK * map.TileSize;
        chunkPos.y = Mathf.Floor(chunkPos.z) * TILES_PER_CHUNK * map.TileSize;
        return chunkPos;
    }
}
