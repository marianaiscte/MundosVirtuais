using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumerado dos tiles possiveis
public enum TileType
{
    Village,
    Forest,
    Plain,
    Desert,
    Sea,
    Mountain
}

[System.Serializable]
public class Tile
{
    public TileType type;
    public GameObject gameO;

    // Inicializa o tile
    public Tile(TileType tileType)
    {
        type = tileType;
    }

    // associa um cubo
    public void associateCube(GameObject cube){
        gameO = cube;
    }

    public GameObject getGameO(){
        return gameO;
    }
}
