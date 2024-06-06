using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    //Guarda a referência ao objeto que será movido
    GameObject movingObject;

    //posição alvo
    Vector3 targetPos;

    //Guarda o componente Animator do objeto em movimento
    Animator animate;

    // Função para iniciar o movimento do objeto
    public void StartMoving(GameObject movingObject, Vector3 targetPos)
    {
        this.movingObject = movingObject;
        this.targetPos = targetPos;
        animate = movingObject.GetComponent<Animator>();
        animate.SetBool("moving",true);
        
    }

    // Método FixedUpdate é chamado em intervalos fixos de tempo
    public void FixedUpdate()
    {
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPos, 1 * Time.deltaTime); // move o objeto na direcao pertendida
           if (movingObject.transform.position == targetPos) // se chegou à posicao final coloca o boolean que controlar o movimento a falso
        {
            animate.SetBool("moving", false);
        }
    }
}
