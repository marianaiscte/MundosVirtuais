using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game{

    public string name;
    public Board board;
    public Player[] roles;
    //public List<Unit[]> turns = new List<Unit[]>();
    public Unit[][] turns;
    public int nTurns;

    public Game(Board gameboard, Player[] gameroles, int numberTurns, string name){
        board = gameboard;
        roles = gameroles;
        nTurns = numberTurns;
        turns = new Unit[nTurns][];
        name = name;
    }

}

