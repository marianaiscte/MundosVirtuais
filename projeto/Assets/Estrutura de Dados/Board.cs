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

// Função que inicializa as casas do tabuleiro com o material suposto
public void InitializeTiles(GameObject boardGameObject){

    Material desertMat = Resources.Load<Material>("Materials/Desert");
    Material forestMat = Resources.Load<Material>("Materials/Forest");
    Material plainMat = Resources.Load<Material>("Materials/Plain");
    Material seaMat = Resources.Load<Material>("Materials/Sea");
    Material villageMat = Resources.Load<Material>("Materials/Village");
    Material mountainMat = Resources.Load<Material>("Materials/Mountain");
        
    Vector3 posicaoTabuleiro = boardGameObject.transform.position;
    Quaternion rotacaoTabuleiro = boardGameObject.transform.rotation;
        
    // Obtém a escala local do objeto
    Vector3 escala = boardGameObject.transform.localScale;

    // Tamanho de cada cubo
    float xC = escala.x / this.Height;
    float zC = escala.z / this.Width;

    // Cria um objeto pai para os cubos
    GameObject tilesParent = new GameObject("CubosParent");
    // Define o objeto pai como filho do tabuleiro
    tilesParent.transform.SetParent(boardGameObject.transform);
    // Ajusta a posição local do objeto pai
    tilesParent.transform.localPosition = Vector3.zero;

    // Loop pelos tiles do tabuleiro
    for (int x = 0; x < this.Width; x++)
    {
        for (int y = 0; y < this.Height; y++)
        {
            // Obtém a casa na posição (x, y)
            Tile tile = this.BoardDisplay[x, y];    

            // Calcula a posição do cubo em relação ao tabuleiro
            Vector3 posicaoCubo = new Vector3(
                -escala.x / 2f + (xC / 2f) + y * xC, 
                0,
                -escala.z / 2f + (zC / 2f) + x * zC
            ) ;

            // Cria o objeto que vai corresponder à casa
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(xC, 0.05f, zC); 
            cube.transform.SetParent(tilesParent.transform); 
            // Define a posição local do cubo em relação ao objeto pai
            cube.transform.localPosition = posicaoCubo; 

            tile.associateCube(cube);

            // Define o material do cubo com base no tipo de tile
            Renderer renderer = cube.GetComponent<Renderer>();
            switch (tile.type)
            {
                case TileType.Village:
                    renderer.material = villageMat;
                    break;
                case TileType.Forest:
                    renderer.material = forestMat;
                    break;
                case TileType.Plain:
                    renderer.material = plainMat;
                    break;
                case TileType.Sea:
                    renderer.material = seaMat;
                    break;
                case TileType.Desert:
                    renderer.material = desertMat;
                    break;
                case TileType.Mountain:
                    renderer.material = mountainMat;
                    break;
            }
        }
    }

    // Chama a função BorderMaker
    BorderMaker(escala, "left", posicaoTabuleiro, tilesParent);
    BorderMaker(escala, "right", posicaoTabuleiro, tilesParent);
    BorderMaker(escala, "up", posicaoTabuleiro, tilesParent);
    BorderMaker(escala, "down", posicaoTabuleiro, tilesParent);

    // Ajusta a posição e rotação do objeto pai
    tilesParent.transform.position = posicaoTabuleiro;
    tilesParent.transform.rotation = rotacaoTabuleiro;
}

// Função que cria as bordas do tabuleiro consoante o seu tamanho 
public void BorderMaker(Vector3 scale, string wallType, Vector3 boardPosition, GameObject tilesParent) {
    GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
    Vector3 wallScale;
    Vector3 wallPositionOffset;
    
    // Cria o tamanho de cada lado e sua posição
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
    wallRenderer.material.color = Color.black;
}



}
