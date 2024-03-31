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
}
