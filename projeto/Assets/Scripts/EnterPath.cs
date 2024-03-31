using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterPath : MonoBehaviour
{
    //public InputField path;
    public Button XMLLoader;

    // Start is called before the first frame update
    void Start()
    {
        path.gameObject.SetActive(false);
        
    }


    void OnMouseDown()
    {
        if (XMLLoader != null)
        {
            path.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
