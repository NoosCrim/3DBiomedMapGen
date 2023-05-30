using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TreeEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;

[System.Serializable]
public class MapGenData
{
    public TextureAtlas TexAtlas;
    public string Name = "new Map";
    public float TileSize = 2.0f;
    public float NoiseScale = 20;
    public float NoiseImpact = 20;
    public float BiomeMargin = 1.01f;
    public List<BiomeIDSize> Biomes;
    public bool EnforceFirstBiome;
    public int Seed = 0;
}
[System.Serializable]
public class Map
{
    private int MAX_BIOME_TRY_COUNT = 100;
    const int MAX_BIOME_PARENT_SWAP_COUNT = 100;
    public readonly string Name;
    public readonly float TileSize;
    private readonly float NoiseScale;
    private readonly float NoiseImpact;
    private readonly float TileTolerance;
    public readonly List<Biome> Biomes;
    private readonly (BiomeType Type, float Size)[] BiomeGenData;
    public readonly Dictionary<Vector2, Chunk> Chunks;
    private readonly bool EnforceFirstBiome;
    public readonly int Seed;
    private readonly int SeedX, SeedY;
    public readonly TextureAtlas TexAtlas;
    public readonly System.Random Random;
    public Map(MapGenData data)
    {
        this.Random = new System.Random(data.Seed);
        this.Seed = data.Seed;
        this.SeedX = data.Seed % 1000000;
        this.SeedY = data.Seed / 1000000;
        this.Name = data.Name;
        this.BiomeGenData = new (BiomeType Type, float Size)[data.Biomes.Count];
        this.TileSize = data.TileSize;
        for (int i = 0; i < data.Biomes.Count; i++)
            this.BiomeGenData[i] = (Type: World.BiomeLib[data.Biomes[i].ID], Size: data.Biomes[i].Size);
        this.Biomes = new List<Biome>();
        this.EnforceFirstBiome = data.EnforceFirstBiome;
        this.Chunks = new Dictionary<Vector2, Chunk>();
        this.NoiseScale = data.NoiseScale;
        this.NoiseImpact = data.NoiseImpact;
        this.TileTolerance = data.BiomeMargin;
        this.TexAtlas = data.TexAtlas;
    }

    public Map(string name, (BiomeType Type, float Size)[] genInput, float noiseScale, float noiseImpact, float tileTolerance, int seed, TextureAtlas texAtlas, bool enforceFirstBiome = false)
    {
        this.Random = new System.Random(seed);
        this.Seed = seed;
        this.SeedX = seed % 1000000;
        this.SeedY = seed / 1000000;
        this.Name = name;
        this.BiomeGenData = genInput;
        this.Biomes = new List<Biome>();
        this.EnforceFirstBiome = enforceFirstBiome;
        this.Chunks = new Dictionary<Vector2, Chunk>();
        this.NoiseScale = noiseScale;
        this.NoiseImpact = noiseImpact;
        this.TileTolerance = tileTolerance;
        this.TexAtlas = texAtlas;
    }
    private float Perlin(Vector2 v)
    {
        return Mathf.PerlinNoise(SeedX + v.x, SeedY + v.y);
    }
    private float GetBiomeWeight(Vector2 tile, Biome biome)
    {
        return (tile + new Vector2(Perlin(tile/NoiseScale), Perlin(new Vector2(tile.x + 2137, tile.y + 69)/NoiseScale)) * NoiseImpact - biome.PosVector).magnitude / (biome.Size);
    }
    private float RandRange(float min, float max)
    {
        return (float)this.Random.NextDouble() * (max - min) + min;
    }
    private int RandRange(int min, int max)
    {
        return this.Random.Next(min, max);
    }
    Vector2 GetTilePos(Vector2 p, Map map)
    {
        return new Vector2(Mathf.Floor(p.x / map.TileSize), Mathf.Floor(p.y / map.TileSize)) * map.TileSize;
    }
    Vector2 RandomUnitVector()
    {
        float rand = RandRange(0.0f, 360.0f);
        return new Vector2(Mathf.Cos(rand), Mathf.Sin(rand));
    }
    Biome PlaceBiome(BiomeType type, float size, Biome parent)
    {
        Biome placed = null;
        Vector2 pos = parent.PosVector + RandomUnitVector() * (parent.Size + size);
        bool goodPos = false;
        for(int tryCount = 0; tryCount < MAX_BIOME_TRY_COUNT && !goodPos; tryCount ++)
        {
            goodPos = true;
            foreach(Biome biome in Biomes)
            {
                if((biome.PosVector - pos).magnitude + Biome.OVERLAP_TOLERANCE <= biome.Size + size)
                {
                    goodPos = false;
                    break;
                }
            }
        }
        if(goodPos)
        {
            Biomes.Add(placed = new Biome(type, pos.x, pos.y, size));
            parent.Neighbours.Add(placed);
            placed.Neighbours.Add(parent);
        }
            
        return placed;
        
    }
    Biome PlaceBiome(BiomeType type, float size, Vector2 pos)
    {
        Biome placed = null;
        foreach (Biome biome in Biomes)
        {
            if ((biome.PosVector - pos).magnitude < biome.Size + size)
            {
                return null;
            }
        }
        Biomes.Add(placed = new Biome(type, pos.x, pos.y, size));
        return placed;
    }
    void addTile(Biome biome, Vector2 pos, Map map)
    {
        //Debug.Log(pos);
        Vector2 chunkPos = Chunk.getChunkPos(pos, this);
        if (!Chunks.ContainsKey(chunkPos))
        {
            Chunks.Add(chunkPos, new Chunk(chunkPos, this, Chunks.Count));
        }
        Chunk chunk = Chunks[chunkPos];
        Tile newTile = new Tile(pos, biome, chunk, this, chunk.Tiles.Count);
        Chunks[chunkPos].Tiles.Add(newTile);
        biome.Tiles.Add(pos, newTile);
        
        int vi0 = chunk.tempVertices.Count;
        Vector3 worldPos = new Vector3(pos.x, 0.0f, pos.y);
        chunk.tempVertices.Add(worldPos);
        chunk.tempVertices.Add(new Vector3(worldPos.x + map.TileSize, 0, worldPos.z));
        chunk.tempVertices.Add(new Vector3(worldPos.x, 0, worldPos.z + map.TileSize));
        chunk.tempVertices.Add(new Vector3(worldPos.x + map.TileSize, 0, worldPos.z + map.TileSize));

        int texRow = biome.BiomeType.TexAtlasIndex / TexAtlas.Size;
        int texCol = biome.BiomeType.TexAtlasIndex % TexAtlas.Size;
        Vector2 tileTexStart = new Vector2(texCol * TexAtlas.TileStep + TexAtlas.Padding, texRow * TexAtlas.TileStep + TexAtlas.Padding);
        chunk.tempUVs.Add(tileTexStart);
        chunk.tempUVs.Add(tileTexStart + new Vector2(TexAtlas.TileSize, 0.0f));
        chunk.tempUVs.Add(tileTexStart + new Vector2(0.0f, TexAtlas.TileSize));
        chunk.tempUVs.Add(tileTexStart + new Vector2(TexAtlas.TileSize, TexAtlas.TileSize));

        chunk.tempTriangles.Add(vi0);
        chunk.tempTriangles.Add(vi0 + 2);
        chunk.tempTriangles.Add(vi0 + 1);

        chunk.tempTriangles.Add(vi0 + 1);
        chunk.tempTriangles.Add(vi0 + 2);
        chunk.tempTriangles.Add(vi0 + 3);
    }
    private bool GenBiomeMap()
    {
        Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;
        Biomes.Clear();
        int[] genOrder = new int[BiomeGenData.Length];
        for (int i = 0; i < genOrder.Length; i++)
            genOrder[i] = i;
        if (EnforceFirstBiome)
            RandRange(0,1);
        for (int i = EnforceFirstBiome ? 1 : 0; i < genOrder.Length - 1; i++)
        {
            int rand = RandRange(i, genOrder.Length - 1);
            int t = genOrder[rand];
            genOrder[rand] = genOrder[i];
            genOrder[i] = t;
        }

        PriorityQueue<Biome, float> open = new PriorityQueue<Biome, float>();
        HashSet<Biome> closed = new HashSet<Biome>();
        open.Enqueue(PlaceBiome(BiomeGenData[genOrder[0]].Type, BiomeGenData[genOrder[0]].Size, new Vector2(0.0f, 0.0f)), (float)Random.NextDouble());

        for (int j = 1; j < genOrder.Length; j++)
        {
            int index = genOrder[j];
            Biome placedBiome = null;
            Biome parent = null;
            for (int i = 0; i < MAX_BIOME_PARENT_SWAP_COUNT && placedBiome == null; i++)
            {
                parent = open.Dequeue();
                open.Enqueue(parent, (float)Random.NextDouble());

                placedBiome = PlaceBiome(BiomeGenData[index].Type, BiomeGenData[index].Size, parent);

                if (placedBiome == null)
                {
                    closed.Add(parent);
                }
                else
                    open.Enqueue(placedBiome, (float)Random.NextDouble());
            }
            if (placedBiome == null)
                return false;
            min.x = placedBiome.PosVector.x < min.x ? placedBiome.PosVector.x : min.x;
            min.y = placedBiome.PosVector.y < min.y ? placedBiome.PosVector.y : min.y;
            max.x = placedBiome.PosVector.x > max.x ? placedBiome.PosVector.x : max.x;
            max.y = placedBiome.PosVector.y > max.y ? placedBiome.PosVector.y : max.y;
            
        }
        Vector2 biomeOffset = (max + min) / 2.0f;
        foreach (Biome biome in Biomes)
        {
            biome.pos.x -= biomeOffset.x;
            biome.pos.y -= biomeOffset.y;
        }
        return true;
    }
    private void GenTilesNMesh()
    {
        HashSet<Vector2> closed = new HashSet<Vector2>();
        Queue<Vector2> open = new Queue<Vector2>();
        Vector2[] neighbours =
        {
            new Vector2(TileSize, 0.0f),
            new Vector2(-TileSize, 0.0f),
            new Vector2(0.0f, TileSize),
            new Vector2(0.0f, -TileSize)
        };
        foreach (Biome biome in Biomes)
        {
            
            Vector2 p = GetTilePos(biome.PosVector, this);
            open.Enqueue(p);
            closed.Add(p);
        }
            
        while(open.Count > 0)
        {
            Vector2 tilePos = open.Dequeue();
            closed.Add(tilePos);
            float biomeWeight = float.PositiveInfinity;
            Biome closestBiome = null;
            foreach(Biome biome in Biomes)
            {
                float w = GetBiomeWeight(tilePos, biome);
                if(w < biomeWeight)
                {
                    biomeWeight = w;
                    closestBiome = biome;
                }
            }
            if (biomeWeight < TileTolerance)
            {
                addTile(closestBiome, tilePos, this);
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (!closed.Contains(tilePos + neighbours[i]))
                    {
                        open.Enqueue(tilePos + neighbours[i]);
                        closed.Add(tilePos + neighbours[i]);
                    }
                        
                }
            }

        }
        foreach(var chunk in Chunks)
        {
            chunk.Value.Mesh.vertices = chunk.Value.tempVertices.ToArray();
            chunk.Value.Mesh.triangles = chunk.Value.tempTriangles.ToArray();
            chunk.Value.Mesh.uv = chunk.Value.tempUVs.ToArray();
            chunk.Value.Mesh.RecalculateNormals();
            chunk.Value.Mesh.RecalculateBounds();
        }
    }
    void GenStructures()
    {
        foreach(Biome biome in Biomes)
        {
            foreach(var tile in biome.Tiles)
            {
                foreach(var structure in biome.BiomeType.Structures)
                {
                    if(Random.NextDouble() < structure.ProbabilityPerTile)
                    {
                        World.StructureLib[structure.ID].map = this;
                        Vector3 p = tile.Value.RandPosInTile(this);
                        Debug.Log(tile.Key);
                        Debug.Log(p);
                        for (int i = 0; i < Structure.MAX_PLACE_TRY_COUNT && World.StructureLib[structure.ID].Place(this, p, Quaternion.Euler(90.0f, 0.0f, RandRange(0.0f, 360.0f))) == null; i++);
                    }
                }
                
            }
        }
    }
    public bool Generate()
    {
        if (!GenBiomeMap())
            return false;
        GenTilesNMesh();
        GenStructures();
        return true;

    }
}
