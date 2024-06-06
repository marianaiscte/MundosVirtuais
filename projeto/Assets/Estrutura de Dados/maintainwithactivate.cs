using UnityEngine;
using UnityEngine.SceneManagement;


public class PersistentObject : MonoBehaviour
{//classe que permite manter os GameObjects da 1º scene na 2º scene, de modo a serem
//utilizáveis quando voltarmos à primeira scene, este tem a particularidade de os manter
//ativos para se conseguir usar o terrainTypeHolder (GameObject que transfere o tipo de terreno ao TerrainGenerator)
    private static PersistentObject instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Room2")
        {
            gameObject.SetActive(true);
        }
        else if (scene.name == "Parte 2 terrain")
        {
            gameObject.SetActive(false);
        }
    }
}
