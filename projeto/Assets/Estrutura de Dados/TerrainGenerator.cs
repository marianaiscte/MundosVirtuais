using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public string xmlFilePath;
    public Terrain terrain;
    public string terrainType;
    public GameObject parentObject; // GameObject pai para organização
    private TerrainData terrainData;
    private List<Vector3> objectPositions = new List<Vector3>(); // Lista para armazenar posições de todos os objetos
    public float minDistanceHouse = 5f; // Distância mínima entre objetos
    public float minDistance = 3f; // Distância mínima entre objetos


    // Terrain layers for different types
    public TerrainLayer desertLayer;
    public TerrainLayer forestLayer;
    public TerrainLayer mountainGrassLayer;
    public TerrainLayer mountainRockLayer;
    public TerrainLayer villageGrassLayer;
    public TerrainLayer villagePathLayer;
    public TerrainLayer plainLayer;

    void Start()
    {
        terrainData = terrain.terrainData;
        terrainData.terrainLayers = GetTerrainLayersForType(terrainType); // Defina as camadas de textura
        GenerateTerrainFromXML(xmlFilePath, terrainType);
    }

    TerrainLayer[] GetTerrainLayersForType(string terrainType)
    {
        switch (terrainType.ToLower())
        {
            case "desert":
                return new TerrainLayer[] { desertLayer };
            case "forest":
                return new TerrainLayer[] { forestLayer };
            case "mountain":
                return new TerrainLayer[] { mountainGrassLayer, mountainRockLayer };
            case "village":
                return new TerrainLayer[] { villageGrassLayer, villagePathLayer };
            case "plain":
                return new TerrainLayer[] { plainLayer };
            default:
                return new TerrainLayer[] { plainLayer };
        }
    }

    void GenerateTerrainFromXML(string filePath, string terrainType)
    {
        XDocument xmlDoc = XDocument.Load(filePath);
        XElement terrainElement = null;

        foreach (XElement square in xmlDoc.Descendants("square"))
        {
            if (square.Attribute("type").Value == terrainType)
            {
                terrainElement = square;
                break;
            }
        }

        if (terrainElement != null)
        {
            float maxElevation = float.Parse(terrainElement.Attribute("maximum_elevation").Value, System.Globalization.CultureInfo.InvariantCulture);
            GenerateTerrain(maxElevation);
            PaintTerrain(maxElevation, terrainType);
            PlaceObjects(terrainElement, maxElevation);
        }
        else
        {
            Debug.LogError("Terrain type not found in XML");
        }
    }

    void GenerateTerrain(float maxElevation)
    {
        int resolution = terrainData.heightmapResolution;
        float[,] heights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float xCoord = (float)x / resolution;
                float zCoord = (float)z / resolution;
                float noiseValue = Mathf.PerlinNoise(xCoord * 10, zCoord * 10);
                heights[x, z] = noiseValue * maxElevation / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void PaintTerrain(float maxElevation, string terrainType)
    {
        int resolution = terrainData.alphamapResolution;
        float[,,] splatmapData = new float[resolution, resolution, terrainData.terrainLayers.Length];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float worldX = (float)x / resolution * terrainData.size.x;
                float worldZ = (float)y / resolution * terrainData.size.z;
                float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

                float[] splatWeights = new float[terrainData.terrainLayers.Length];

                if (terrainType.ToLower() == "mountain")
                {
                    float rockNoise = Mathf.PerlinNoise(worldX * 0.3f, worldZ * 0.3f); // Scale noise for rock distribution
                    if (height < maxElevation * 0.5f)
                    {
                        splatWeights[0] = 1; // Grass at lower altitudes
                    }
                    else if (rockNoise > 0.5f)
                    {
                        splatWeights[1] = 1; // Smaller patches of rock at higher altitudes
                    }
                    else
                    {
                        splatWeights[0] = 1; // Grass at other high altitudes
                    }
                }
                else if (terrainType.ToLower() == "village")
                {
                    float pathNoise = Mathf.PerlinNoise(worldX * 0.3f, worldZ * 0.3f); // Scale noise for path distribution
                    if (height < maxElevation * 0.2f && pathNoise > 0.5f)
                    {
                        splatWeights[1] = 1; // Smaller patches of path at very low altitudes
                    }
                    else
                    {
                        splatWeights[0] = 1; // Grass elsewhere
                    }
                }
                else
                {
                    splatWeights[0] = 1; // Single texture for desert, forest, and plain
                }

                float total = 0;
                for (int i = 0; i < splatWeights.Length; i++)
                {
                    total += splatWeights[i];
                }

                for (int i = 0; i < splatWeights.Length; i++)
                {
                    splatWeights[i] /= total;
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    void PlaceObjects(XElement terrainElement, float maxElevation)
    {
        float lowAltitudeThreshold = 0.2f * maxElevation;
        float highAltitudeThreshold = 0.8f * maxElevation;

        foreach (XElement obj in terrainElement.Elements("object"))
        {
            string type = obj.Attribute("type").Value;
            string densityLow = obj.Attribute("density_low_altitute").Value;
            float densityLowF = float.Parse(densityLow, System.Globalization.CultureInfo.InvariantCulture);
            string densityHigh = obj.Attribute("density_high_altitute").Value;
            float densityHighF = float.Parse(densityHigh, System.Globalization.CultureInfo.InvariantCulture);

            PlaceObjectOfType(type, densityLowF, densityHighF, lowAltitudeThreshold, highAltitudeThreshold);
        }
    }

    void PlaceObjectOfType(string type, float densityLow, float densityHigh, float lowAltitudeThreshold, float highAltitudeThreshold)
    {
        int resolution = terrainData.heightmapResolution;

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float worldX = (float)x / resolution * terrainData.size.x;
                float worldZ = (float)z / resolution * terrainData.size.z;
                float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

                float density = height < lowAltitudeThreshold ? densityLow :
                                height > highAltitudeThreshold ? densityHigh : 0f;

                if (Random.value < density)
                {
                    Vector3 position = new Vector3(worldX, height, worldZ);

                    // Verificação de distância mínima para todos os objetos
                    if (type == "house" && !IsPositionValidHouse(position))
                    {
                        continue;
                    }

                    if (!IsPositionValid(position))
                    {
                        continue;
                    }

                    GameObject obj = InstantiateObject(type, position);
                    if (obj != null)
                    {
                        obj.transform.parent = parentObject.transform;
                        objectPositions.Add(position); // Armazenar a posição do objeto instanciado
                    }
                }
            }
        }
    }

    bool IsPositionValidHouse(Vector3 position)
    {
        foreach (Vector3 objPos in objectPositions)
        {
            if (Vector3.Distance(objPos, position) < minDistanceHouse)
            {
                return false;
            }
        }
        return true;
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 objPos in objectPositions)
        {
            if (Vector3.Distance(objPos, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    GameObject InstantiateObject(string type, Vector3 position){
        string folderPath = "Terrain/" + type + "/";
        GameObject[] prefabs = Resources.LoadAll<GameObject>(folderPath);
        
        if (prefabs.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
            GameObject objPrefab = prefabs[randomIndex];

            float randomYRotation = UnityEngine.Random.Range(-180, 180);

            Quaternion randomRotation = Quaternion.Euler(
            objPrefab.transform.rotation.eulerAngles.x,
            objPrefab.transform.rotation.eulerAngles.y + randomYRotation, 
            objPrefab.transform.rotation.eulerAngles.z 
            );
            
    
            return Instantiate(objPrefab, position, randomRotation);
        }
        else
        {
            Debug.LogError("No prefabs found in folder: " + folderPath);
            return null;
        }
    }


}