using UnityEngine;
using System;
using System.Xml;
using System.Collections.Generic;

public class XMLReader : MonoBehaviour
{
    //public string xmlFilePath; // Caminho do ficheiro XML

    public Game LoadXMLToRead(string xmlFilePath){
        XmlReader xmlr = XmlReader.Create(xmlFilePath);
        return ReadXML(xmlr);
        /* Carrega o ficheiro XML da pasta Resources (mudar isto)
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

    Game ReadXML(XmlReader xmlr){
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
                        board = DealWithBoard(xmlr);
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
        while (xmlr.Read()){
            string p_name = xmlr.GetAttribute("name");
            Player p = new Player(p_name);
            rolesList.Add(p); 
        }
        return rolesList.ToArray();
    }

    Board DealWithBoard(XmlReader xmlr){
        int width = int.Parse(xmlr.GetAttribute("width"));
        int height = int.Parse(xmlr.GetAttribute("height"));
        Tile[,] tiles = new Tile[width, height];
        int x = 0;
        int y = 0;
        while (xmlr.Read()){
            switch (xmlr.Name){
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
            if(x >= width){
                x = 0;
                y++;
            }
        }

        Board board = new Board(width, height, tiles);

        return board;
        
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

