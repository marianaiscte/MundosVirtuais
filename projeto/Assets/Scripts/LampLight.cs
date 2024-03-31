using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour{
    public Light luz;
    private bool luzOn = false;

    void Start()
    {
        luz.enabled = false;
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
                }
                else
                {
                    luzOn = true;
                    luz.enabled = true;
                }
            }
        }
    }
}
