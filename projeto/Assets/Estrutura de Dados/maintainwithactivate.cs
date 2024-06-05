using UnityEngine;
using UnityEngine.SceneManagement;


public class PersistentObject : MonoBehaviour
{
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
