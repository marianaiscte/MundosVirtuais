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
    public float minDistance = 5f; // Distância mínima entre objetos

    void Start()
    {
        terrainData = terrain.terrainData;
        GenerateTerrainFromXML(xmlFilePath, terrainType);
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

    GameObject InstantiateObject(string type, Vector3 position)
    {
        GameObject objPrefab = Resources.Load<GameObject>("Terrain/" + type);
        if (objPrefab != null)
        {
            return Instantiate(objPrefab, position, objPrefab.transform.rotation); // Mantendo a rotação original do prefab
        }
        else
        {
            Debug.LogError("Prefab not found for type: " + type);
            return null;
        }
    }
}
