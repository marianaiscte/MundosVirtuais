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
    public int id;
    public Player owner;
    public PieceType type;
    public int x;
    public int y;

    public Piece(int pieceId, Player pieceOwner, PieceType pieceType, int posX, int posY)
    {
        id = pieceId;
        owner = pieceOwner;
        type = pieceType;
        x = posX;
        y = posY;
    }
}

