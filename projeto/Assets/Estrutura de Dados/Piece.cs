using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enumerado do conjunto de peças possiveis
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
    public int id { get; private set; } //id da peça
    public Player owner { get; private set; }  //jogador detentor da peça
    public PieceType type { get; private set; }  //tipo da peça
    public int x { get;  set; }  //posição x
    public int y { get;  set; }  //posição y
    // Campo para armazenar o GameObject associado a esta peça
    public GameObject gameO;  
    public Boolean isHolding { get; set; } // indica se a peça está a ser guardada
    public Boolean attacking { get; set; } // indica se a peça está a atacar alguma posicao
    public Game game;
    public List<(int, int)> oldPositions = new List<(int, int)>();  // Lista para armazenar posições anteriores da peça
    
    public Piece(int pieceId, Player pieceOwner, PieceType pieceType, int posX, int posY)
    {
        // Inicializa as propriedades com os valores passados como parâmetros
        id = pieceId;
        owner = pieceOwner;
        type = pieceType;
        x = posX;
        y = posY;
        isHolding = false;  // inicializado como false
        attacking = false;  // inicializado como false
    }

    // Método para associar um GameObject a esta peça
    public void associateObj(GameObject obj){
        gameO = obj;
        // Adiciona um collider  ao objeto para detetar cliques
        BoxCollider collider = gameO.AddComponent<BoxCollider>();
        // Ajustar o tamanho do collider conforme necessário
        collider.center = new Vector3(0, 1, 0);
        collider.size = new Vector3(gameO.transform.localScale.x +1, gameO.transform.localScale.y + 1, gameO.transform.localScale.z + 1); // Define um tamanho padrão para o collider
        //Debug.Log(gameO.transform.localScale);
    
        RouteViewer seeRoute = gameO.AddComponent<RouteViewer>(); // Adiciona um componente RouteViewer ao GameObject
        seeRoute.SetPiece(this);
    }


    public GameObject getGameO(){
        return gameO;
    }
}

