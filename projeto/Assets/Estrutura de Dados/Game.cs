using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game{

    public string name { get;}
    public Board board { get;}
    public Player[] roles { get;}

    public List<Unit[]> turns = new List<Unit[]>();
    
    public int nTurns;

    public Game(Board gameboard, Player[] gameroles, List<Unit[]> turnlist, string givenName){
        board = gameboard;
        roles = gameroles;
        turns = turnlist;
        nTurns = turns.Count;
        name = givenName;
    }

}

