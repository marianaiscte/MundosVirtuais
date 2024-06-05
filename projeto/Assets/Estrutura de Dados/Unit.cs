using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// Enumerado das acoes possíveis
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

    // Construtor da classe Unit
    public Unit(ActionType actionType, Piece pieceInAction, int posX, int posY, string role)
    {
        action = actionType;
        piece = pieceInAction;
        posFocoX = posX;
        posFocoY = posY;
        rolePlaying = role;
    }

    // Método para definir a peça como em "hold"
    public void hold(){
        piece.isHolding = true;
    }
    // Método para o ataque, retornando as coordenadas do foco da ação
    public int[] attack(){
        int[] coordinates = {posFocoX, posFocoY};
        return coordinates;
    }

    // Método para obter uma descrição da unit
    public string getUnit(){
        // Retorna uma string que descreve o tipo da peça, a ação atual e o tipo da peça associada
        return "Tipo: " + this.piece + ", Ação: " + this.action + ", Peça:" + this.piece.type;
        
    }
}
