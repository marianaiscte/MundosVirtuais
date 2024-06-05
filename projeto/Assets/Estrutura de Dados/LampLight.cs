using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LampLight : MonoBehaviour{
    public Light luz;
    private bool luzOn = false;
    public Slider lightSlider;

    // coloca a luz desligada e o slider desligado tambem
    void Start()
    {
        luz.enabled = false;
        lightSlider.gameObject.SetActive(false);
    }

    // Método Update é chamado uma vez por frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica se o raio colidiu com algum objeto e se esse objeto é o mesmo que o gameObject atual
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                // Alterna o estado da luz e do slider
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
        // Se a luz estiver ligada, ajusta a intensidade de acordo com o valor do slider
        if (luzOn){
            luz.intensity = lightSlider.value;
        }
         
    }   
}
