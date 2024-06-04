using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public static SceneController instance;

    public string mainSceneName = "Room2"; // Nome da cena principal
    public string cutsceneSceneName = "Parte 2 terrain"; // Nome da cena da cutscene

       void Awake(){
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

    void Start()
    {
        // Carrega o estado da cena principal se esta é a cena principal
        if (SceneManager.GetActiveScene().name == mainSceneName)
        {
            SceneSaver.instance.LoadSceneState();
        }
    }

    public void ChangeScene()
    {
        SceneSaver.instance.SaveSceneState();
        SceneManager.LoadScene(cutsceneSceneName, LoadSceneMode.Single);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnMainSceneLoaded;
    }

    private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainSceneName)
        {
            SceneSaver.instance.LoadSceneState();
            SceneManager.sceneLoaded -= OnMainSceneLoaded;
        }
    }
}
