using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class World
{
    public readonly static List<Map> Maps = new List<Map>();
    public readonly static List<BiomeType> BiomeLib = new List<BiomeType>();
    public readonly static List<Structure> StructureLib = new List<Structure>();
}
