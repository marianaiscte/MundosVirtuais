using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeskScript : MonoBehaviour
{
    public Button XMLLoader; // Reference to the button you want to activate

    public bool gameIsPlaying = false;

    void Start()
    {
        XMLLoader.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if(!gameIsPlaying){// Activate the first button when the object is clicked
        Debug.Log("clicaste na mesa");
        XMLLoader.gameObject.SetActive(true);
        gameIsPlaying = true;
        }
        
    }

}

