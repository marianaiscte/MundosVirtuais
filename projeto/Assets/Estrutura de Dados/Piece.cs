using System;
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
    public int x { get;  set; }
    public int y { get;  set; }
    public GameObject gameO;
    public Boolean isHolding { get; set; }
    public Boolean attacking { get; set; }
    
    public Piece(int pieceId, Player pieceOwner, PieceType pieceType, int posX, int posY)
    {
        id = pieceId;
        owner = pieceOwner;
        type = pieceType;
        x = posX;
        y = posY;
        isHolding = false;
        attacking = false;
    }

    public void associateObj(GameObject obj){
        gameO = obj;
    }

    public GameObject getGameO(){
        return gameO;
    }
}

