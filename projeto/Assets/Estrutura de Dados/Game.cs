using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Game{

    public string name { get;}
    public Board board { get;}
    public Player[] roles { get;}

    public List<Unit[]> turns = new List<Unit[]>();

    public int nTurns;

    public List<Piece> pieces;

    public Game(Board gameboard, Player[] gameroles, List<Unit[]> turnlist, string givenName){
        board = gameboard;
        roles = gameroles;
        turns = turnlist;
        nTurns = turns.Count;
        name = givenName;
        pieces = new List<Piece>();
    }

    public void addPiece(Piece piece){
        pieces.Add(piece);
    }

    public void UpdatePosPiece(Piece piece, int newX, int newY){
    if (pieces.Contains(piece)){
        piece.x = newX;
        piece.y = newY;

    }
    Debug.Log("Peça "+ piece.id + " moveu-se para x = "+piece.x+ " e y =" +piece.y);

    }

}

