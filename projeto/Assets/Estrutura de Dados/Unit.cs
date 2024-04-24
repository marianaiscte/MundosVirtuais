using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log("AAAAAAAAAAAAAAAAAAAAAA ");

        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Renderer renderer = cyl.GetComponent<Renderer>();

        GameObject gameTile = tile.getGameO();
        
        cyl.transform.position = gameTile.transform.position;
        cyl.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

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

    public void moveTo(){

    }

    public void hold(){

    }


    public void attack(){

    }
}
