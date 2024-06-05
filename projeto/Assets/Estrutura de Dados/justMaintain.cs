using UnityEngine;
using UnityEngine.SceneManagement;


public class maintain : MonoBehaviour
{
    private static maintain instance;

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

}