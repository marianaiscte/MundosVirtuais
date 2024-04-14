using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board 
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Tile[,] BoardDisplay;


    public Board(int width, int height, Tile[,] boardDisplay)
    {
        Width = width;
        Height = height;
        BoardDisplay = boardDisplay;
    }

}
