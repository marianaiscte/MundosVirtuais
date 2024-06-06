using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LampLight : MonoBehaviour{
    public Light luz;
    private bool luzOn = false;

    // coloca a luz desligada e o slider desligado tambem
    void Start()
    {
        luz.enabled = false;
    }

    // Método Update é chamado uma vez por frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Cria um raio a partir da posição do rato a partir da câmera principal
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
                }
                else
                {
                    luzOn = true;
                    luz.enabled = true;
                }
            }
        }
        // Se a luz estiver ligada, ajusta a intensidade de acordo com o valor do slider
         
    }   
}
