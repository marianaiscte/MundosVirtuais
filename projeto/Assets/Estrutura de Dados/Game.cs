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

    public List<Piece> pieces {get;}

    public Game(Board gameboard, Player[] gameroles, List<Unit[]> turnlist, string givenName){
        board = gameboard;
        roles = gameroles;
        turns = turnlist;
        nTurns = turns.Count;
        name = givenName;
        pieces = new List<Piece>();
    }

    public void addPiece(Piece piece){
        piece.game = this;
        pieces.Add(piece);
    }

    public void UpdatePosPiece(Piece piece, int newX, int newY){
    if (pieces.Contains(piece)){
        piece.x = newX;
        piece.y = newY;

    }
    Debug.Log("Peça "+ piece.id + " moveu-se para x = "+piece.x+ " e y =" +piece.y);

    }

    public int CountPiecesInTile(int x, int y){
        int count = 0;
        foreach (Piece p in pieces){
            if(p.x == x && p.y == y){
                count++;
            }
        }
    return count;
    }

    public GameObject[] getObjectsInTile(int x, int y){
        int n = CountPiecesInTile(x, y);
        GameObject[] objects = new GameObject[n];
        for (int i = 0; i < n; i++){
            foreach (Piece p in pieces){
                if(p.x == x && p.y == y){
                    objects[i] = p.getGameO();
                }
            }
        }
        return objects;
    }

    public List<GameObject> getAllObjects(){
        List<GameObject> allPieces = new List<GameObject>();
        for (int i = 0; i < pieces.Count; i++){
            foreach (Piece p in pieces){
                allPieces[i] = p.getGameO();
            }
        }
        return allPieces;
    }

    
    public List<Piece> getPiecesInTile(int x, int y){
        int n = CountPiecesInTile(x, y);
        List<Piece> allPiecesInTile = new List<Piece>();
        for (int i = 0; i < n; i++){
            foreach (Piece p in pieces){
                if(p.x == x && p.y == y){
                    allPiecesInTile.Add(p);
                }
            }
        }
        return allPiecesInTile;
    }

    
    public void SaveOldPositions(List<Dictionary<(int, int), List<Piece>>> oldTurnsPositions){
        foreach(Piece piece in pieces){
            List<(int, int)> oldPositions = new List<(int, int)>();
            foreach (Dictionary<(int, int), List<Piece>> turnPositions in oldTurnsPositions)
            {
                foreach (var kvp in turnPositions)
                {
                    if (kvp.Value.Contains(piece))
                    {
                        oldPositions.Add(kvp.Key);
                    }
                }
            }
            piece.oldPositions = oldPositions;
        }
    }
}

