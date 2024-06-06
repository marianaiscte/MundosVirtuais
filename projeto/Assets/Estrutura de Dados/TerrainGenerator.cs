using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

//função que trata de gerar o terreno da parte 2 do trabalho (e instanciar os espadachins e o personagem principal)
public class TerrainGenerator : MonoBehaviour
{
    //O próprio terrain a ser gerado
    public Terrain terrain;
   //GameObject onde ficarão todos os prefabs de ambiente
    public GameObject parentObject; 
    //TerrainData do Terrain
    private TerrainData terrainData;
    //Lista da posição de todos os prefabs de ambiente
    private List<Vector3> objectPositions = new List<Vector3>(); 
    //distancia mínima entre casas (de modo a evitar sobreposições)
    public float minDistanceHouse = 5f; 
    public float minDistance = 3f; // Distância mínima entre objetos (de modo a evitar sobreposições)
    public float flattenRadius = 50f; // Raio da área central a ser terraplanada
// Terrain Layers para todos os tipos de terreno e uma para a terraplana    public TerrainLayer desertLayer;
    public TerrainLayer desertLayer;
    public TerrainLayer forestLayer;
    public TerrainLayer mountainLayer;
    public TerrainLayer villageLayer;
    public TerrainLayer plainLayer;
    public TerrainLayer flatAreaLayer;


//função de Start onde sao feitas as associações essencias e a busca do terrainType a gerar ao 
//GameObject "terrainType" que tem um terrainTypeHolder
    void Start()
    {
        terrainData = terrain.terrainData;
        GameObject gameObjectTerrain = GameObject.Find("terrainType");
        string terrainType = gameObjectTerrain.GetComponent<terrainTypeHolder>().terrainType.ToLower();
        terrainData.terrainLayers = GetTerrainLayersForType(terrainType);//Definição das Layers
        TextAsset[] resources = Resources.LoadAll<TextAsset>("xml"); //busca do ficheiro xml com as
        //elevações e densidade de objetos
         if (resources.Length > 0)
        {
            TextAsset firstXml = resources[0] as TextAsset;
            string resourceName = firstXml.name;
            string resourcePath = "Resources/xml/" + resourceName;
            string xmlContent = firstXml.text;
            Debug.Log("Xml encontrado: " + resourcePath);
            GenerateTerrainFromXML(xmlContent, terrainType);         
        }
        else
        {
            Debug.Log("Nenhum recurso encontrado na pasta: Resources/xml");
        }
        
    }

   //função que retorna a matriz de terrainLayers com base no tipo de terreno especificado
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
    //função "principal" -> recebe o conteúdo do xml e o terrainType e gera o terreno baseando-se nisso
    void GenerateTerrainFromXML(string xmlContent, string terrainType)
    {
        XDocument xmlDoc = XDocument.Parse(xmlContent);
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
             //elevação máxima do terreno
            float maxElevation = float.Parse(terrainElement.Attribute("maximum_elevation").Value, System.Globalization.CultureInfo.InvariantCulture);
            //função que gera o terreno com base na maxElevation com Perlin Noise
            GenerateTerrain(maxElevation);
            //função que pinta o terrain de acordo com as Layers
            PaintTerrain();
            //função que coloca os prefabs de ambiente (house,rock e tree)
            PlaceObjects(terrainElement, maxElevation);
            //função que planifica a area toda
            FlattenCentralArea();    
            //função que coloca o main character e os espadachins no terreno
            PlaceAttackerAndDefenderAndChar();
        }else
        {
            Debug.LogError("Terrain type not found in XML");
        }
    }



    //função que gera o terreno com base na maxElevation com Perlin Noise
    void GenerateTerrain(float maxElevation)
    {
        int resolution = terrainData.heightmapResolution;
        float[,] heights = new float[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                float xCoord = (float)x / resolution; //coordenadas normalizadas
                float zCoord = (float)z / resolution;
                float noiseValue = Mathf.PerlinNoise(xCoord * 10, zCoord * 10); //frequência de 10
                heights[x, z] = noiseValue * maxElevation / terrainData.size.y; //normalizar a altura
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

//função que pinta o terreno de acordo com as terrainLayers (default de cada terreno em todo o lado menos no local plano)
    void PaintTerrain(){
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
            float height = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));//altura do ponto

            float[] splatWeights = new float[terrainData.terrainLayers.Length]; 

            float distance = Mathf.Sqrt(Mathf.Pow(worldX - centerX, 2) + Mathf.Pow(worldZ - centerZ, 2));

            if (distance <= radius)
            {//pertence à arena
                for (int i = 0; i < splatWeights.Length; i++)
                {
                    splatWeights[i] = 2;
                }
            }
            else
            {// Layer Default
                splatWeights[0] = 1; 
            }

            float total = 0;
             //Os pesos são somados e depois normalizados para que a soma dos pesos em cada ponto seja 1
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

//função que coloca todos os objetos/prefabs de ambiente (rock,tree e house) de acordo com um Threshold
//definido pelo grupo de modo a evitar sobrecarregamento
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
//função que coloca um objeto/prefab especifico (tree ou house ou rock) de acordo com a minima distancia entre outros objetos
    void PlaceObjectOfType(string type, float densityLow, float densityHigh, float lowAltitudeThreshold, float highAltitudeThreshold){
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
//função que valida se a posição é valida para uma house (para ser colocada uma casa tem de ter uma distancia de 5 com o objeto
//mais próximo)
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
//função que valida se a posição é valida para qualquer outro objeto (sem ser uma house) 
//(para serem colocados estes tem de ter uma distancia mínima de 3)
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

    //função que trata de "planificar" a arena (círculo no centro do terrain)
    void FlattenCentralArea(){
        int resolution = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, resolution, resolution);

        int centerX = resolution / 2;//coordenada X do centro
        int centerZ = resolution / 2;//coordenada Z do centro

        float centerHeight = heights[centerX, centerZ]; //altura do centro
        //Calcula o raio da área que será nivelada, 
        float flattenRadiusInHeightmapSpace = flattenRadius / terrainData.size.x * resolution;
        //raio de suavização para suavizar a transição entre a área nivelada e o terreno a sua volta
        float smoothingRadius = flattenRadiusInHeightmapSpace * 0.2f;
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                //Calcula a distância entre o ponto atual e o centro do terreno
                float distance = Mathf.Sqrt((x - centerX) * (x - centerX) + (z - centerZ) * (z - centerZ));

                if (distance < flattenRadiusInHeightmapSpace)
                { //Se o ponto estiver dentro da área a ser nivelada, sua altura é definida como a altura do centro do terreno.
                    heights[x, z] = centerHeight;
                }
                else if (distance < flattenRadiusInHeightmapSpace + smoothingRadius)
                {//Se o ponto estiver dentro da zona de suavização, a altura do ponto é interpolada entre a 
                //altura do centro do terreno e sua altura original, usando uma função de suavização 
                    float t = (distance - flattenRadiusInHeightmapSpace) / smoothingRadius;
                    t = Mathf.SmoothStep(0, 1, t);  // Suaviza a interpolação
                    heights[x, z] = Mathf.Lerp(centerHeight, heights[x, z], t);
                }
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }



//função que instancia o objeto em si, de acordo com a posição e com uma rotaçao em y aleatória
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

    //função que coloca o mainCharacter, o espadachim Attacker e o Defender em cena 
    void PlaceAttackerAndDefenderAndChar(){
        Vector3 centerPosition = new Vector3(terrainData.size.x / 2, 0, terrainData.size.z / 2);
        centerPosition.y = terrain.SampleHeight(centerPosition);
        Vector3 attackerpos = new Vector3(centerPosition.x-1,centerPosition.y,centerPosition.z);
        Vector3 defenderPosition = new Vector3(centerPosition.x+1,centerPosition.y,centerPosition.z);
        Vector3 charpos = new Vector3(centerPosition.x,centerPosition.y,230);
        GameObject attackerPrefab = Resources.Load<GameObject>("Mixamo swordWoman/Attacker");
        GameObject defenderPrefab = Resources.Load<GameObject>("Mixamo swordWoman/Defender");
        GameObject charPrefab = Resources.Load<GameObject>("Mixamo swordWoman/Main char");
        
        if (attackerPrefab != null)
        {
            GameObject attacker = Instantiate(attackerPrefab, attackerpos, attackerPrefab.transform.rotation);
            attacker.transform.parent = parentObject.transform;
        }
        else
        {
            Debug.LogError("Attacker prefab not found.");
        }

        if (defenderPrefab != null)
        {
            GameObject defender = Instantiate(defenderPrefab, defenderPosition,  defenderPrefab.transform.rotation);
            defender.transform.parent = parentObject.transform;
        }
        else
        {
            Debug.LogError("Defender prefab not found.");
        }

        if (charPrefab != null)
        {
            GameObject mainChar = Instantiate(charPrefab, charpos, charPrefab.transform.rotation);
            
        }
        else
        {
            Debug.LogError("Main char prefab not found.");
        }

    }

}