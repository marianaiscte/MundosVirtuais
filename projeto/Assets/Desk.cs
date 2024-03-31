using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeskScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        button.gameObject.SetActive(false);
    }


    public Button button; // Referência para o botão que queres ativar

    void OnMouseDown()
    {
        // Ativa o botão quando o objeto é clicado
        button.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
