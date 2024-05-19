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
    public Game game;
    public List<(int, int)> oldPositions = new List<(int, int)>();
    
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
        // Adicione um componente Collider (por exemplo, um Collider de caixa) ao objeto para detectar cliques
        BoxCollider collider = gameO.AddComponent<BoxCollider>();
            // Ajustar o tamanho do collider conforme necessário
        collider.center = new Vector3(0, 1, 0);
        collider.size = new Vector3(gameO.transform.localScale.x +1, gameO.transform.localScale.y + 1, gameO.transform.localScale.z + 1); // Defina um tamanho padrão para o collider
        Debug.Log(gameO.transform.localScale);
        
        RouteViewer seeRoute = gameO.AddComponent<RouteViewer>();
        seeRoute.SetPiece(this);
    }


    public GameObject getGameO(){
        return gameO;
    }
}

