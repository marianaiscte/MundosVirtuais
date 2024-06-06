using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject topViewCamera;
    public GameObject miniMap;

    // Lida com a mudança de camera e com o minimapa
    public void changeCamera(){
        // Se a camera principal estiver ativa, esta é desativada e é ativada a camera superior à mesa e o minimapa tambem fica visivel
        if(mainCamera.activeSelf)
        {
            mainCamera.SetActive(false);
            topViewCamera.SetActive(true);
            miniMap.SetActive(true);
        }
        // alteracoes inversas
        else
        {
            mainCamera.SetActive(true);
            topViewCamera.SetActive(false);
            miniMap.SetActive(false);
        }
    }

}
