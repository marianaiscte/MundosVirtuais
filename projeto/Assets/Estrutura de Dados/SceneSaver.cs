using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SceneState
{
    public List<ObjectState> objectStates = new List<ObjectState>();
}

[System.Serializable]
public class ObjectState
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public ObjectState(string _name, Vector3 _position, Quaternion _rotation)
    {
        name = _name;
        position = _position;
        rotation = _rotation;
    }
}

public class SceneSaver : MonoBehaviour
{
    public static SceneSaver instance;

    void Awake()
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

    public void SaveSceneState()
    {
        SceneState sceneState = new SceneState();

        GameObject[] objectsInScene = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objectsInScene)
        {
        
                ObjectState objectState = new ObjectState(obj.name, obj.transform.position, obj.transform.rotation);
                sceneState.objectStates.Add(objectState);
        
        }

        string json = JsonUtility.ToJson(sceneState);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "sceneState.json"), json);
    }

    public void LoadSceneState()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "sceneState.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SceneState sceneState = JsonUtility.FromJson<SceneState>(json);

            foreach (ObjectState objectState in sceneState.objectStates)
            {
                GameObject obj = GameObject.Find(objectState.name);
                if (obj != null)
                {
                    obj.transform.position = objectState.position;
                    obj.transform.rotation = objectState.rotation;
                }
            }
        }
    }
}
