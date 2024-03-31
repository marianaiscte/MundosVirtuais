using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Tile(TileType tileType)
    {
        type = tileType;
    }
}
