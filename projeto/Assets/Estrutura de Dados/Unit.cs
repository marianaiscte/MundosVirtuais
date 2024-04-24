using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public enum ActionType
{
    spawn,
    move_to,
    hold,
    attack
}

[System.Serializable]
public class Unit
{
    public ActionType action { get; private set; }
    public Piece piece { get; private set; }
    public int posFocoX { get; private set; }
    public int posFocoY { get; private set; }

    public Unit(ActionType actionType, Piece pieceInAction, int posX, int posY)
    {
        action = actionType;
        piece = pieceInAction;
        posFocoX = posX;
        posFocoY = posY;
    }

    public void spawn(Board board){

        int x = this.posFocoX - 1;
        int y = this.posFocoY - 1;

        Debug.Log(x + " " + y);

        Tile tile = board.BoardDisplay[x, y];

        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Renderer renderer = cyl.GetComponent<Renderer>();

        GameObject gameTile = tile.getGameO();
        
        cyl.transform.position = gameTile.transform.position;
        cyl.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        piece.associateObj(cyl);
        Debug.Log(piece.getGameO());

        switch(this.piece.type.ToString()){
            
            case "Soldier":
            renderer.material.color = Color.black;
            break;

            case"Archer":
            renderer.material.color = Color.green;
            break;

            case "Mage":
            renderer.material.color = Color.cyan;
            break;

            case"Catapult":
            renderer.material.color = Color.gray;
            break;

        }

    }

    public void moveTo(Board board){

        int x = this.posFocoX - 1;
        int y = this.posFocoY - 1;

        Tile tile = board.BoardDisplay[x, y];

        GameObject mover = piece.getGameO();
        Debug.Log(mover);

        GameObject gameTile = tile.getGameO();
        Debug.Log(gameTile);



        Vector3 targetPos = gameTile.transform.position;

        mover.transform.position = Vector3.MoveTowards(mover.transform.position, targetPos, 4 * Time.deltaTime);

        //mover.transform.position = targetPos;
        
    }

    public void hold(){

    }


    public void attack(){

    }
}
