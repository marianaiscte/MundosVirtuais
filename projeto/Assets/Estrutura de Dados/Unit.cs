using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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

    public string rolePlaying { get; private set; }

    public Unit(ActionType actionType, Piece pieceInAction, int posX, int posY, string role)
    {
        action = actionType;
        piece = pieceInAction;
        posFocoX = posX;
        posFocoY = posY;
        rolePlaying = role;
    }

    public void hold(){
        piece.isHolding = true;
    }

    public int[] attack(){
        int[] coordinates = {posFocoX, posFocoY};
        return coordinates;
    }

    public string getUnit(){
        return "Tipo: " + this.piece + ", Ação: " + this.action + ", Peça:" + this.piece.type;
    }
}
