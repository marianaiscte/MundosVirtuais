using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeskScript : MonoBehaviour
{
    public Button XMLLoader; // Referência ao botão que vai ser ativado

    public bool gameIsPlaying = false; // Booleano que serve para o utilizador não ativar o botão a meio de um jogo

    void Start()
    {
        XMLLoader.gameObject.SetActive(false);
    }

    // Quando o objeto é clicado
    void OnMouseDown()
    {
        if(!gameIsPlaying){
            XMLLoader.gameObject.SetActive(true);
            gameIsPlaying = true;
        }
        
    }

}

