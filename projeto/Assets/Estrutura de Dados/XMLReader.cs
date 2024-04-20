using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class XMLReader : MonoBehaviour
{
    public InputField xmlFilePath; // Caminho do ficheiro XML
    //public GameObject boardGameObject; // Referência para o GameObject do tabuleiro no Unity Editor
    public GameObject villageTilePrefab; // Prefab do tile da vila
    public GameObject forestTilePrefab;
    public GameObject  plainTilePrefab;
    public GameObject desertTilePrefab;
    public GameObject seaTilePrefab;
    public GameObject mountainTilePrefab;



public Game LoadXMLToRead(string xmlFilePath, GameObject boardGameObject){        
        XmlReader xmlr = XmlReader.Create(xmlFilePath);
        return ReadXML(xmlr, boardGameObject);        /* Carrega o ficheiro XML da pasta Resources (mudar isto)
        TextAsset xmlAsset = Resources.Load<TextAsset>(xmlFilePath);

        if (xmlAsset != null){
            // Cria um novo leitor XML
            XmlReader xmlr = XmlReader.Create(new System.IO.StringReader(xmlAsset.text));
        
            // Chama uma função para processar o XML
            return ReadXML(xmlr);
        }else {
            Debug.LogError("Ficheiro XML não encontrado: " + xmlFilePath);
            return null;
            }*/
    }

    Game ReadXML(XmlReader xmlr, GameObject boardGameObject){
        //coisas para checkar DTD

        string game_name = ""; // Declare outside the switch statement
        Player[] roles = null;
        Board board = null;
        List<Unit[]> allTurns = new List<Unit[]>();

        while(xmlr.Read()){
            //switch case para escolher o que fazer com cada tag?? Se calhar ter uma função para cada tipo de tag, senão aqui fica too much
            //todos eles vão ter de entrar dentro dos seus child -> pôr mais um loop nessas funções para percorrer filhos?

            if (xmlr.NodeType == XmlNodeType.Element){
                switch (xmlr.Name){
                    case "game": 
                        game_name = DealWithGame(xmlr); 
                        break;

                    case "roles": 
                        roles = DealWithRoles(xmlr);
                        break;
                    
                    case "board": 
                        board = DealWithBoard(xmlr, boardGameObject);
                        break;
                    
                    case "turns": 
                        allTurns = DealWithTurns(xmlr);
                        break;
                }
            }

        }
        Game game = new Game(board, roles, allTurns, game_name); 
        return game;
    }

    string DealWithGame(XmlReader xmlr){
        return xmlr.GetAttribute("name");
    }

    Player[] DealWithRoles(XmlReader xmlr){
        List<Player> rolesList = new List<Player>();
        
        // Move o leitor para o primeiro elemento <role> dentro de <roles>
        if (xmlr.ReadToDescendant("role")) {
            do {
                // Processa o elemento <role> atual
                string p_name = xmlr.GetAttribute("name");
                Player p = new Player(p_name);
                rolesList.Add(p);
            } while (xmlr.ReadToNextSibling("role")); // Move para o próximo irmão <role>, se houver
            
        }
        
        return rolesList.ToArray();
    }


    Board DealWithBoard(XmlReader xmlr, GameObject boardGameObject){
        int width = int.Parse(xmlr.GetAttribute("width"));
        int height = int.Parse(xmlr.GetAttribute("height"));
        Tile[,] tiles = new Tile[width, height];
        int x = 0;
        int y = 0;
        //Pôr tudo a partir daqui mais bonito e compreensível pq isto é maioritariamente do chat e está hardcoded

        //ele estava a ler mal as tags, lia coisas vaizas potanto com isto só lê o que é suposto
        List<string> expectedTags = new List<string> { "village", "forest", "plain", "sea", "desert", "mountain" };

        while (xmlr.Read()) {
            if (xmlr.NodeType == XmlNodeType.Element) {
                Debug.Log("Entrei");
                Debug.Log(xmlr.Name);
                // Verifica se a tag atual está na lista de tags esperadas
                if (expectedTags.Contains(xmlr.Name)) {
            switch (xmlr.Name) {
                    case "village":
                        tiles[x, y] = new Tile(TileType.Village);
                        break;
                    case "forest":
                        tiles[x, y] = new Tile(TileType.Forest);
                        break;
                    case "plain":
                        tiles[x, y] = new Tile(TileType.Plain);
                        break;
                    case "sea":
                        tiles[x, y] = new Tile(TileType.Sea);
                        break;
                    case "desert":
                        tiles[x, y] = new Tile(TileType.Desert);
                        break;
                    case "mountain":
                        tiles[x, y] = new Tile(TileType.Mountain);
                        break;
                }
                x++;
                if (x >= width) {
                    x = 0;
                    y++;
                }
                if (y >= height) {
                    break; // Termina o loop quando todos os elementos da matriz foram preenchidos
                }
                }
            }
        }
            Debug.Log("Conteúdo da matriz tiles:");

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Debug.Log("tiles[" + i + ", " + j + "]: " + tiles[i, j]);
                }
            }
            Board board = new Board(width, height, tiles);
            InitializeTiles(board, boardGameObject);
            return board;
        }

        void InitializeTiles(Board board, GameObject boardGameObject){
        
        Vector3 posicaoTabuleiro = boardGameObject.transform.position;
        Quaternion rotacaoTabuleiro = boardGameObject.transform.rotation;
        Vector3 tamanhoTabuleiro = boardGameObject.GetComponent<Renderer>().bounds.size;

        float escalaX = tamanhoTabuleiro.x / board.Width;
        float escalaZ = tamanhoTabuleiro.z / board.Height;

        GameObject tilesParent = new GameObject("CubosParent");
        //criei para tentar fazer a rotaçao de todos os cubos no final, em vez de fazer um a um que envolvia mais
        //calculos


        // Loop pelos tiles do tabuleiro
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                // Obtém o tile na posição (x, y)
                Tile tile = board.BoardDisplay[x, y];    

                // calcula a posiçao tendo em conta os "espaços entre cubos" se nao tivesse isto da escala os cubos
                //isto ficavam cubos a flutuar e nao juntinhos
                Vector3 posicaoCubo = posicaoTabuleiro + new Vector3(x * escalaX, 0, y * escalaZ); 

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                cube.transform.localScale = new Vector3(escalaX, (float)0.25, escalaZ);
                
                // Define a posição do cubo com base na posição do tile no tabuleiro
                cube.transform.position = posicaoCubo; 

                cube.transform.parent = tilesParent.transform;
                
                //Meti primeiro os cubos com cores pra ser mais facil
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
        //roda o tilesparent de modo a conseguir rodar todos os cubos
        tilesParent.transform.rotation = Quaternion.Inverse(rotacaoTabuleiro);

    }


    List<Unit[]> DealWithTurns(XmlReader xmlr){

        List<Unit[]> turnsList = new List<Unit[]>();

            while (xmlr.ReadToFollowing("turn")){
                List<Unit> unitsInTurn = new List<Unit>();

                if (xmlr.ReadToDescendant("unit")){
                    do{
                        int id = int.Parse(xmlr.GetAttribute("id"));
                        string role = xmlr.GetAttribute("role");
                        PieceType type = (PieceType)Enum.Parse(typeof(PieceType), xmlr.GetAttribute("type"), true);
                        ActionType action = (ActionType)Enum.Parse(typeof(ActionType), xmlr.GetAttribute("action"), true);
                        int x = int.Parse(xmlr.GetAttribute("x"));
                        int y = int.Parse(xmlr.GetAttribute("y"));

                        Piece piece = new Piece(id, new Player(role), type, x, y);

                        Unit unit = new Unit(action, piece, x, y);

                        unitsInTurn.Add(unit);
                    } while (xmlr.ReadToNextSibling("unit"));
                }
                turnsList.Add(unitsInTurn.ToArray());
            }
            return turnsList;
    }

}

