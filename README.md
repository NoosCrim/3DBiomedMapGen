# 3DBiomedMapGen
simple Unity3D script used to generate flat map divided into biomes. It's based on voronoi diagrams and perlin noise.

![Screenshot 2023-05-30 201354](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/3727229d-2efe-4807-bf32-156f5e9a07c5)
![Screenshot 2023-05-30 201443](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/2cae7155-97af-4f57-aec5-56d1643ef6e8)
![Screenshot 2023-05-30 201458](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/8e392fcb-d125-4109-855e-43fa6f9741f5)


# How to use
1. Copy and paste all the files into your project.
2. Add the script to a GameObject on the scene.
3. Set the settings.![Screenshot 2023-05-30 201458](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/d4473029-1a5d-467b-a288-d978e8c45526)

4. Call ```World.Maps[i].Generate()``` to generate i-th map you put in the settings.

![image](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/fdfa80c3-c094-4c0f-a999-0c521d8c3f00)

Tex Atlas is square texture atlas that will be used in map generation. 

  -Size sets how many squares is atlas divided into
  
  -Padding Size sets how big part of texture is a padding. It's values should be from 0 to 1.
  
  -This is example texture album with it's texture indices written on it:
  
![image](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/b02615c3-96f7-444f-a7ad-a5cc729cec70)
Name sets the name of the map

Tile Size sets how big are tiles used in generation in Unity units

Noise scale scales perlin noise used for generation

Noise Impact determines how much will perlin noise affect the generation. Lower values make biomes have more circular shapes, while bigger ones make them more irregular

Biome margin determines how far away can tile be from biome's center to still be considered a part of the biome.


Biomes is a list of pairs of biome IDs and biome sizes. 

-Biome IDs are indices of Biome Lib in which instructions regarding generation of particular biomes is stored. 

-Size is biome radius in Unity units.


Biome Lib is list storing informations about which structures should generate on which biomes, and how often should they do it.

-Name sets biome lib entry name

-Tex Atlas Index is index of texture that should be used for tiles of the biome.  

-Structures is a list of Structure IDs and probabilities

--ID is index of structure data stored in Structure Lib

--Probability Per Tile is probability of particular structure apearing on a tile 

Structure Lib is a list of structure components, where data about particular structure is stored. To fill fields of this list making prefabs of structures is needed. Those prefabs must contain Structure component

![image](https://github.com/NoosCrim/3DBiomedMapGen/assets/133163547/cb5640bd-44c2-4e32-95b7-7ed06e6e68ee)

Mesh is structure mesh

Material is structure material

Radius is radius in which other structures cannot appear while generating.

