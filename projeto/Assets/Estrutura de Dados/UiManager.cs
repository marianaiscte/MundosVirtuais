using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject topViewCamera;
    private bool cKeyPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !cKeyPressed){
            Debug.Log("C pressed");
            changeCamera();
            cKeyPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            cKeyPressed = false;
        }
        
    }

    public void changeCamera(){
        if(mainCamera.activeSelf)
        {
            mainCamera.SetActive(false);
            topViewCamera.SetActive(true);
        }
        else
        {
            mainCamera.SetActive(true);
            topViewCamera.SetActive(false);
        }
    }

}
