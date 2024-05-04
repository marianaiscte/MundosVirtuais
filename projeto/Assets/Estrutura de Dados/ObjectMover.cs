using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    GameObject movingObject;

    Vector3 targetPos;

    Animator animate;


    // Start is called before the first frame update
    public void StartMoving(GameObject movingObject, Vector3 targetPos)
    {
        this.movingObject = movingObject;
        this.targetPos = targetPos;
        animate = movingObject.GetComponent<Animator>();
        animate.SetBool("moving",true);
        
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPos, 1 * Time.deltaTime);
           if (movingObject.transform.position == targetPos)
        {
            animate.SetBool("moving", false);
        }
    }
}
