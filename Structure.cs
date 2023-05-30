using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StructureIDProbability
{
    public int ID;
    public float ProbabilityPerTile;
}
public class Structure : MonoBehaviour
{
    public const int MAX_PLACE_TRY_COUNT = 10;
    public const int STRUCTURE_LAYER = 3;
    public Map map;
    [SerializeField]
    private Mesh mesh;
    public Mesh Mesh
    {
        get
        {
            return mesh;
        }
        set
        {
            mesh = value;
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
    [SerializeField]
    private Material material;
    public Material Material
    {
        get
        {
            return material;
        }
        set
        {
            material = value;
            GetComponent<MeshRenderer>().material = material;
        }
    }
    [SerializeField]
    private float radius;
    public float Radius
    {
        get
        {
            return radius;
        }
        set
        {
            radius = value;
        }
    }
    bool CanPlace(/*Map map,*/ Vector3 pos)
    {
        /*Vector2 chunkPos = Chunk.getChunk(pos);
        Chunk[] toCheck =
        {
            map.Chunks[pos],
            map.Chunks[pos + new Vector2(Tile.Size, 0.0f)],
            map.Chunks[pos + new Vector2(0.0f, Tile.Size)],
            map.Chunks[pos + new Vector2(Tile.Size, Tile.Size)],
            map.Chunks[pos + new Vector2(-Tile.Size, 0.0f)],
            map.Chunks[pos + new Vector2(0.0f, -Tile.Size)],
            map.Chunks[pos + new Vector2(-Tile.Size, -Tile.Size)],
            map.Chunks[pos + new Vector2(Tile.Size, -Tile.Size)],
            map.Chunks[pos + new Vector2(-Tile.Size, Tile.Size)]
        };*/
        Vector2 cPos = Chunk.getChunkPos(pos, map);
        if(map.Chunks.ContainsKey(cPos + new Vector2(-1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(-1, -1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(0, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(0, -1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(1, -1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;

        if (map.Chunks.ContainsKey(cPos + new Vector2(-1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(-1, 0) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(0, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(0, 0) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(1, 0) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;

        if (map.Chunks.ContainsKey(cPos + new Vector2(-1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(-1, 1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(0, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(0, 1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;
        if (map.Chunks.ContainsKey(cPos + new Vector2(1, -1) * Chunk.TILES_PER_CHUNK))
            foreach (Structure structure in map.Chunks[cPos + new Vector2(1, 1) * Chunk.TILES_PER_CHUNK].Structures)
                if ((pos - structure.gameObject.transform.position).sqrMagnitude > (radius + structure.radius) * (radius + structure.radius))
                    return false;

        return true;
    }
    public GameObject Place(Map map, Vector3 pos, Quaternion rot)
    {
        if(CanPlace(pos))
        {
            GameObject newGO = new GameObject();
            newGO.SetActive(false);
            Structure newStructure = newGO.AddComponent<Structure>();
            newStructure.mesh = mesh;
            newStructure.material = material;
            newStructure.radius = radius;
            newStructure.map = map;
            newGO.AddComponent<MeshFilter>().mesh = mesh;
            newGO.AddComponent<SphereCollider>().radius = radius;
            newGO.AddComponent<MeshRenderer>().material = material;
            newGO.transform.position = pos;
            newGO.transform.rotation = rot;
            newGO.layer = STRUCTURE_LAYER;
            newGO.SetActive(true);
            map.Chunks[Chunk.getChunkPos(pos, map)].Structures.Add(newStructure);
            return newGO;
        }
        return null;
    }
    void copyData(Structure source)
    {
        Mesh = source.mesh;
        Radius = source.radius;
        this.Material = source.material;
        map = source.map;
    }
    void Start()
    {
        if (gameObject.GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        if (gameObject.GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = material;
    }

}
