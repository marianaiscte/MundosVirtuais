using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Tile[,] boardDisplay;


    public Board(int width, int height)
    {
        Width = width;
        Height = height;
        boardDisplay = new Tile[Width, Height];
    }

    //public void setTile(int X, int Y, Tile tile){}

    //public Tile getTile(int X, int Y){}

}
