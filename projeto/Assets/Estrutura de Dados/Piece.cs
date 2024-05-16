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
        collider.size = new Vector3(1, 1, 1);

        // Adicione um script para lidar com cliques
        RouteViewer seeRoute = gameO.AddComponent<RouteViewer>();

        // Defina o sprite que você deseja associar
        Sprite mySprite = Resources.Load<Sprite>("Assets/footsteps_9069966.png"); // Substitua "Sprites/mySprite" pelo caminho do seu sprite

        // Passe uma referência para este objeto para o script de manipulador de cliques
        seeRoute.SetPieceAndSprite(this, mySprite);
    }


    public GameObject getGameO(){
        return gameO;
    }
}

