using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldControler : MonoBehaviour
{
    public List<MapGenData> Maps = new List<MapGenData>();
    public List<BiomeType> BiomeLib = new List<BiomeType>();
    public List<Structure> StructureLib = new List<Structure>();
    void Start()
    {
        foreach (var structure in StructureLib)
        {
            World.StructureLib.Add(structure);
        }
        foreach (var biomeType in BiomeLib)
        {
            World.BiomeLib.Add(biomeType);
        }
        foreach (var mapData in Maps)
        {
            World.Maps.Add(new Map(mapData));
        }
        //foreach (Map map in World.Maps)
        //    map.Generate();
    }

    
    void Update()
    {
        
    }
}
