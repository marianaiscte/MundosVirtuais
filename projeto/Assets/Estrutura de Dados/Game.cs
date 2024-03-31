using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game{
    public Board board;
    public Player[] roles;
    //public List<Unit[]> turns = new List<Unit[]>();
    public Unit[][] turns;
    public int nTurns;
    public int nUnits; //se cada turn tiver um numero diferente de units temos de alterar

    public Game(Board gameboard, Player[] gameroles, int numberTurns, int nUnitsPerTurn){
        board = gameboard;
        roles = gameroles;
        nTurns = numberTurns;
        nUnits = nUnitsPerTurn;
        turns = new Unit[nTurns][];
    }

    /* Add novo turno 
    public void AddTurn(Unit[] units)
    {
        turns.Add(units);
    }*/

}

