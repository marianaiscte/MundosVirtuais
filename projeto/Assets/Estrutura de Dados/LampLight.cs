using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LampLight : MonoBehaviour{
    public Light luz;
    private bool luzOn = false;
    public Slider lightSlider;

    void Start()
    {
        luz.enabled = false;
        lightSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                
                if (luzOn)
                {
                    luzOn = false;
                    luz.enabled = false;
                    lightSlider.gameObject.SetActive(false);
                }
                else
                {
                    luzOn = true;
                    luz.enabled = true;
                    lightSlider.gameObject.SetActive(true);
                }
            }
        }
        if (luzOn){
            luz.intensity = lightSlider.value;
        }
         
    }   
}
