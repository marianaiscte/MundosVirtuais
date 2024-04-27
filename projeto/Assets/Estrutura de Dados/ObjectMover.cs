using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    GameObject movingObject;

    Vector3 targetPos;

    // Start is called before the first frame update
    public void StartMoving(GameObject movingObject, Vector3 targetPos)
    {
        this.movingObject = movingObject;
        this.targetPos = targetPos;
        Debug.Log("posição" + targetPos);
        Debug.Log("obj" + movingObject);
    }

    // Update is called once per frame
    public void Update()
    {
        movingObject.transform.position = Vector3.MoveTowards(movingObject.transform.position, targetPos, 4 * Time.deltaTime);
    }
}
