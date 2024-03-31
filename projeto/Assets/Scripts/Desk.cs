using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeskScript : MonoBehaviour
{
    public Button XMLLoader; // Reference to the button you want to activate

    void Start()
    {
        XMLLoader.gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        // Activate the first button when the object is clicked
        XMLLoader.gameObject.SetActive(true);
    }

}

