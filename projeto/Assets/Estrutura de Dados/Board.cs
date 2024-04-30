using System;
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

    public void InitializeTiles(GameObject boardGameObject){
        
        Vector3 posicaoTabuleiro = boardGameObject.transform.position;
        Quaternion rotacaoTabuleiro = boardGameObject.transform.rotation;
        
        Vector3 tamanhoExato = boardGameObject.GetComponent<Renderer>().bounds.size;
        //Debug.Log(tamanhoExato);

        // Obtém a escala local do objeto
        Vector3 escala = boardGameObject.transform.localScale;

        //tamanho de cada cubo
        float xC = escala.x / this.Height;
        float zC = escala.z / this.Width;

        //Debug.Log(xC + ", " + zC);

        GameObject tilesParent = new GameObject("CubosParent");

        tilesParent.transform.SetParent(boardGameObject.transform);
        tilesParent.transform.position = boardGameObject.transform.localPosition;

        // Loop pelos tiles do tabuleiro
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                // Obtém o tile na posição (x, y)
                Tile tile = this.BoardDisplay[x, y];    

                   Vector3 posicaoCubo = new Vector3(
                    posicaoTabuleiro.x - escala.x/2f + (xC/2f) + y * xC, 
                    posicaoTabuleiro.y,
                    posicaoTabuleiro.z - escala.z/2f + (zC/2f) + x * zC
                    ) ;
            // Cria o cubo
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(xC, 0.05f, zC); 
            cube.transform.SetParent(tilesParent.transform); 
            cube.transform.position = posicaoCubo; 

            tile.associateCube(cube);

            // Define a cor do cubo com base no tipo de tile
            Renderer renderer = cube.GetComponent<Renderer>();
            switch (tile.type)
                {
                    case TileType.Village:
                        renderer.material.color = Color.black;
                        break;
                    case TileType.Forest:
                        renderer.material.color = Color.green;
                        break;
                    case TileType.Plain:
                        renderer.material.color = Color.yellow;
                        break;
                    case TileType.Sea:
                        renderer.material.color = Color.blue;
                        break;
                    case TileType.Desert:
                        renderer.material.color = Color.yellow;
                        break;
                    case TileType.Mountain:
                        renderer.material.color = Color.gray;
                        break;
                }
            }
        }

        BorderMaker(escala, "left", posicaoTabuleiro, tilesParent);
        BorderMaker(escala, "right", posicaoTabuleiro, tilesParent);
        BorderMaker(escala, "up", posicaoTabuleiro, tilesParent);
        BorderMaker(escala, "down", posicaoTabuleiro, tilesParent);
        tilesParent.transform.position = posicaoTabuleiro;
        tilesParent.transform.rotation = rotacaoTabuleiro;
    }

    public void BorderMaker(Vector3 scale, string wallType, Vector3 boardPosition, GameObject tilesParent) {
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Vector3 wallScale;
    Vector3 wallPositionOffset;
    
    switch (wallType) {
        case "left":
            wallScale = new Vector3(scale.x + 0.1f, 0.07f, 0.05f);
            wallPositionOffset = new Vector3(0f, 0f, scale.z / 2f + 0.025f);
            break;
        case "right":
            wallScale = new Vector3(scale.x + 0.1f, 0.07f, 0.05f);
            wallPositionOffset = new Vector3(0f, 0f, -scale.z / 2f - 0.025f);
            break;
        case "up":
            wallScale = new Vector3(0.05f, 0.07f, scale.z);
            wallPositionOffset = new Vector3(-scale.x / 2f - 0.025f, 0f, 0f);
            break;
        case "down":
            wallScale = new Vector3(0.05f, 0.07f, scale.z);
            wallPositionOffset = new Vector3(scale.x / 2f + 0.025f, 0f, 0f);
            break;
        default:
            return; // Não faz nada se o tipo de parede não for reconhecido
    }
    
    wall.transform.localScale = wallScale;
    wall.transform.position = boardPosition + wallPositionOffset;
    wall.transform.SetParent(tilesParent.transform);
    
    Renderer wallRenderer = wall.GetComponent<Renderer>();
    wallRenderer.material.color = Color.white;
}



}
