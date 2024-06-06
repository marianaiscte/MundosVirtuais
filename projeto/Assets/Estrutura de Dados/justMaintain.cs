using UnityEngine;
using UnityEngine.SceneManagement;

//classe que permite manter os GameObjects da 1º scene na 2º scene, de modo a serem
//utilizáveis quando voltarmos à primeira scene
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