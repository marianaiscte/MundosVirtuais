using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public string xmlFilePath;
    public Terrain terrain;
    public string terrainType;
    public GameObject parentObject; 
    private TerrainData terrainData;
    private List<Vector3> objectPositions = new List<Vector3>(); 
    public float minDistanceHouse = 5f; 
    public float minDistance = 3f; // Distância mínima entre objetos
    public float flattenRadius = 50f; // Raio da área central a ser terraplanada



    // Terrain layers for different types
    public TerrainLayer desertLayer;
    public TerrainLayer forestLayer;
    public TerrainLayer mountainLayer;
    public TerrainLayer villageLayer;
    public TerrainLayer plainLayer;
    public TerrainLayer flatAreaLayer;



    void Start()
    {
        terrainData = terrain.terrainData;
        terrainData.terrainLayers = GetTerrainLayersForType(terrainType); // Defina as camadas de textura
        GenerateTerrainFromXML(xmlFilePath, terrainType);
    }

    void FlattenCentralArea(){
        int resolution = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, resolution, resolution);

        int centerX = resolution / 2;
        int centerZ = resolution / 2;

        float centerHeight = heights[centerX, centerZ];

        float flattenRadiusInHeightmapSpace = flattenRadius / terrainData.size.x * resolution;

        float smoothingRadius = flattenRadiusInHeightmapSpace * 0.2f;

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (z - centerZ) * (z - centerZ));

                if (distance < flattenRadiusInHeightmapSpace)
                {
                    heights[x, z] = centerHeight;
                }
                else if (distance < flattenRadiusInHeightmapSpace + smoothingRadius)
                {
                    float t = (distance - flattenRadiusInHeightmapSpace) / smoothingRadius;
                    t = Mathf.SmoothStep(0, 1, t);  // Suaviza a interpolação
                    heights[x, z] = Mathf.Lerp(centerHeight, heights[x, z], t);
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }


        TerrainLayer[] GetTerrainLayersForType(string terrainType)
        {
            switch (terrainType.ToLower())
            {
                case "desert":
                    return new TerrainLayer[] { desertLayer, flatAreaLayer};
                case "forest":
                    return new TerrainLayer[] { forestLayer,flatAreaLayer };
                case "mountain":                
                    return new TerrainLayer[] { mountainLayer,flatAreaLayer};
                case "village":
                    return new TerrainLayer[] { villageLayer,flatAreaLayer};
                case "plain":
                    return new TerrainLayer[] { plainLayer,flatAreaLayer };
                default:
                    return new TerrainLayer[] { plainLayer, flatAreaLayer };
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
            FlattenCentralArea();
            AddRandomGrassToTerrain();
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

    float centerX = terrainData.size.x / 2;
    float centerZ = terrainData.size.z / 2;
    float radius = 50.0f;


    for (int y = 0; y < resolution; y++)
    {
        for (int x = 0; x < resolution; x++)
        {
            float worldX = (float)x / resolution * terrainData.size.x;
            float worldZ = (float)y / resolution * terrainData.size.z;
            float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

            float[] splatWeights = new float[terrainData.terrainLayers.Length];

            float distance = Mathf.Sqrt(Mathf.Pow(worldX - centerX, 2) + Mathf.Pow(worldZ - centerZ, 2));

            if (distance <= radius)
            {
                for (int i = 0; i < splatWeights.Length; i++)
                {
                    splatWeights[i] = 2;
                }
            }
            else
            {
                splatWeights[0] = 1; // Default layer
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

        float centerX = terrainData.size.x / 2;
        float centerZ = terrainData.size.z / 2;
        float radius = 50.0f;

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float worldX = (float)x / resolution * terrainData.size.x;
                float worldZ = (float)z / resolution * terrainData.size.z;
                float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

                float density = height < lowAltitudeThreshold ? densityLow :
                                height > highAltitudeThreshold ? densityHigh : 0f;

                float distance = Mathf.Sqrt(Mathf.Pow(worldX - centerX, 2) + Mathf.Pow(worldZ - centerZ, 2));

                if (distance <= radius){
                    continue;
                }else{

                    if (Random.value < density)
                    {
                        Vector3 position = new Vector3(worldX, height, worldZ);

                        if (type == "house" && !IsPositionValidHouse(position))
                        {
                            continue;
                        }

                        if (type != "house" && !IsPositionValid(position))
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

    void AddRandomGrassToTerrain()
{
    Texture2D[] grassTextures = Resources.LoadAll<Texture2D>("Terrain/grass");

    if (grassTextures.Length == 0)
    {
        Debug.LogError("Nenhuma textura de relva encontrada na pasta Resources/Terrain/grass");
        return;
    }

    Debug.Log("Número de texturas de relva encontradas: " + grassTextures.Length);

    DetailPrototype[] detailPrototypes = new DetailPrototype[grassTextures.Length];
    for (int i = 0; i < grassTextures.Length; i++)
    {
        DetailPrototype grassDetail = new DetailPrototype();
        grassDetail.prototypeTexture = grassTextures[i];

        // Calcula a escala da textura de grama com base na resolução da textura
        float scale = 1.0f / grassTextures[i].width; // Uma maneira simples de calcular a escala
        grassDetail.minWidth = scale * 2; // Ajuste conforme necessário
        grassDetail.maxWidth = scale * 4; // Ajuste conforme necessário
        grassDetail.minHeight = scale * 2; // Ajuste conforme necessário
        grassDetail.maxHeight = scale * 4; // Ajuste conforme necessário

        detailPrototypes[i] = grassDetail;
    }

    terrainData.detailPrototypes = detailPrototypes;
    int detailResolution = terrainData.detailResolution;
    int[,] detailLayer = new int[detailResolution, detailResolution];
    for (int y = 0; y < detailResolution; y++)
    {
        for (int x = 0; x < detailResolution; x++)
        {
            for (int i = 0; i < grassTextures.Length; i++)
            {
                if (Random.value < 0.6f) 
                {
                    detailLayer[x, y] = i; 
                    break;
                }
            }
        }
    }

    terrainData.SetDetailLayer(0, 0, 0, detailLayer);
}



}