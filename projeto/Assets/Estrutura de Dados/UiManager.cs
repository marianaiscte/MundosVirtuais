using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject topViewCamera;
    public GameObject miniMap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.C) && !cKeyPressed){
            Debug.Log("C pressed");
            changeCamera();
        }
        
        */
    }

    public void changeCamera(){
        if(mainCamera.activeSelf)
        {
            mainCamera.SetActive(false);
            topViewCamera.SetActive(true);
            miniMap.SetActive(true);
        }
        else
        {
            mainCamera.SetActive(true);
            topViewCamera.SetActive(false);
            miniMap.SetActive(false);
        }
    }

}
