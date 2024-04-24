using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    Soldier,
    Archer,
    Mage,
    Catapult
}

[System.Serializable]
public class Piece
{
    public int id { get; private set; }
    public Player owner { get; private set; }
    public PieceType type { get; private set; }
    public int x { get; private set; }
    public int y { get; private set; }
    public GameObject gameO;

    public Piece(int pieceId, Player pieceOwner, PieceType pieceType, int posX, int posY)
    {
        id = pieceId;
        owner = pieceOwner;
        type = pieceType;
        x = posX;
        y = posY;
    }

    public void associateObj(GameObject obj){
        gameO = obj;
    }

    public GameObject getGameO(){
        return gameO;
    }
}

