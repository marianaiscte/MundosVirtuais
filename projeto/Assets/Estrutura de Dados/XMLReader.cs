using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Xml.Schema;
using System.IO;
using System.Xml.Resolvers;


public class XMLReader : MonoBehaviour
{
    public InputField xmlFilePath; // Caminho do ficheiro XML
    public GameObject BoardGameObject; // Referência para o GameObject do tabuleiro no Unity Editor
    public GameObject villageTilePrefab; // Prefab do tile da vila
    public GameObject forestTilePrefab;
    public GameObject  plainTilePrefab;
    public GameObject desertTilePrefab;
    public GameObject seaTilePrefab;
    public GameObject mountainTilePrefab;
    public event Action<Game> OnGameLoaded;

    Dictionary<int, Piece> pieceDictionary = new Dictionary<int, Piece>();

    public Game LoadXMLToRead(string xmlFilePath, GameObject boardGameObject) {     
 
        
        string xmlContent = File.ReadAllText(xmlFilePath);
        //Debug.Log(xmlContent);
        string dtdContent = File.ReadAllText("Assets/Resources/dtd/play-out.dtd"); 
        //Debug.Log(dtdContent);
        
        File.WriteAllText("Assets/Resources/dtd/tempXml.xml", xmlContent); // escreve o conteudo do xml num ficheiro temporario

        XmlReaderSettings settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Parse,
            ValidationType = ValidationType.DTD,
            //IgnoreWhitespace = true // Isso instrui o XmlReader a ignorar nós de espaço em branco
        };
        Debug.Log(settings==null);
        settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallback); //handler para validacao

        XmlReader xmlr = XmlReader.Create("Assets/Resources/dtd/tempXml.xml", settings); // leitor com as configuracoes especificadas
        BoardGameObject = boardGameObject;
        return ReadXML(xmlr, boardGameObject);  
           
    }

    private void ValidationCallback(object sender, ValidationEventArgs args) {
        if (args.Severity == XmlSeverityType.Warning)
            Debug.LogWarning("Warning: " + args.Message);
        else if (args.Severity == XmlSeverityType.Error)
            Debug.LogError("Error: " + args.Message);
        }

    public Game ReadXML(XmlReader xmlr, GameObject boardGameObject){


        string game_name = ""; // nome do jogo
        Player[] roles = null;
        Board board = null;
        List<Unit[]> allTurns = new List<Unit[]>();

        //le o xml ate ao final
        while(xmlr.Read()){
            //Debug.Log(xmlr.Name);

            if (xmlr.NodeType == XmlNodeType.Element){
                //Debug.Log(xmlr.Name);
                // Decide o que fazer com base no nome do elemento e processa-o adequadamente
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
                        allTurns = DealWithTurns(xmlr,board);
                        //Debug.Log(allTurns);
                        break;
                }
            }

        }
        // Cria um novo objeto Game com os dados lidos
        Game game = new Game(board, roles, allTurns, game_name); 
        return game;
    }

    //retorna o nome do jogo
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
        //le os atributos largura e altura
        int width = int.Parse(xmlr.GetAttribute("width"));
        int height = int.Parse(xmlr.GetAttribute("height"));
        // cria uma matriz de tiles
        Tile[,] tiles = new Tile[width, height];
        int x = 0;
        int y = 0;

        List<string> expectedTags = new List<string> { "village", "forest", "plain", "sea", "desert", "mountain" };

        while (xmlr.Read()) {
            if (xmlr.NodeType == XmlNodeType.Element) {
                //Debug.Log("Entrei");
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
            //Debug.Log("Conteúdo da matriz tiles:");
            /*
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    //Debug.Log("tiles[" + i + ", " + j + "]: " + tiles[i, j]);
                }
            }*/
            Board board = new Board(width, height, tiles); // Cria um novo objeto Board com as dimensões e a matriz de tiles
            board.InitializeTiles(boardGameObject);
            return board;
        }


    List<Unit[]> DealWithTurns(XmlReader xmlr,Board board){

        List<Unit[]> turnsList = new List<Unit[]>(); //lista de jogadas vazia

            while (xmlr.ReadToFollowing("turn")){ // procura e le cada turn
                List<Unit> unitsInTurn = new List<Unit>(); // cria uma lista para as units em casa turn

                if (xmlr.ReadToDescendant("unit")){ //le ate encontrar a unit
                    do{
                        int id = int.Parse(xmlr.GetAttribute("id"));
                        string role = xmlr.GetAttribute("role");
                        PieceType type = (PieceType)Enum.Parse(typeof(PieceType), xmlr.GetAttribute("type"), true);
                        ActionType action = (ActionType)Enum.Parse(typeof(ActionType), xmlr.GetAttribute("action"), true);
                        int x = int.Parse(xmlr.GetAttribute("x"));
                        int y = int.Parse(xmlr.GetAttribute("y"));

                        Piece piece;
                        
                        if (pieceDictionary.ContainsKey(id)) { //verifica se a peca existe no dicionario
                            piece = pieceDictionary[id];
                        } else {
                            piece = new Piece(id, new Player(role), type, x, y); //se nao adiciona a
                            pieceDictionary.Add(id, piece);
                        }

                        Unit unit = new Unit(action, piece, x, y, role);
                        unitsInTurn.Add(unit); //cria e adiciona uma unit com os dados lidos
                    } while (xmlr.ReadToNextSibling("unit")); //le ate haver units nessa turn
                }
                turnsList.Add(unitsInTurn.ToArray()); // adicona o array de units à lista de turns no turn atual
            }
            return turnsList; //devolve a lista de turns
    }

}

