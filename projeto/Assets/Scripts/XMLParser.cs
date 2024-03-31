using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Acho que aqui é instanciar os objetos de acordo com um xmlfile, já tendo as classes que criei no "estrutura de dados"
        //sinto que  faltam coisas mas estou farta de falar com o chatgpt e de unity :P
        //código do chat:
    //     using UnityEngine;
    //     using System.Collections.Generic;
    //     using System.Xml;

    //     public class XMLParser : MonoBehaviour
    //     {
    //         public TextAsset xmlFile;

    //         private List<Player> players = new List<Player>();
    //         private List<Tile> board = new List<Tile>();
    //         private List<List<Piece>> turns = new List<List<Piece>>();

    //         void Start()
    //         {
    //             ParseXML();
    //         }

    //         void ParseXML()
    //         {
    //             XmlDocument xmlDoc = new XmlDocument();
    //             xmlDoc.LoadXml(xmlFile.text);

    //             XmlNodeList playerNodes = xmlDoc.SelectNodes("/game/roles/role");
    //             foreach (XmlNode playerNode in playerNodes)
    //             {
    //                 string playerName = playerNode.Attributes["name"].Value;
    //                 players.Add(new Player(playerName));
    //             }

    //             XmlNodeList tileNodes = xmlDoc.SelectNodes("/game/board/*");
    //             foreach (XmlNode tileNode in tileNodes)
    //             {
    //                 TileType tileType = (TileType)System.Enum.Parse(typeof(TileType), tileNode.Name, true);
    //                 board.Add(new Tile(tileType));
    //             }

    //             XmlNodeList turnNodes = xmlDoc.SelectNodes("/game/turns/turn");
    //             foreach (XmlNode turnNode in turnNodes)
    //             {
    //                 List<Piece> pieces = new List<Piece>();
    //                 XmlNodeList unitNodes = turnNode.SelectNodes("unit");
    //                 foreach (XmlNode unitNode in unitNodes)
    //                 {
    //                     int pieceId = int.Parse(unitNode.Attributes["id"].Value);
    //                     string playerName = unitNode.Attributes["role"].Value;
    //                     Player player = players.Find(p => p.name == playerName);
    //                     PieceType pieceType = (PieceType)System.Enum.Parse(typeof(PieceType), unitNode.Attributes["type"].Value, true);
    //                     int posX = int.Parse(unitNode.Attributes["x"].Value);
    //                     int posY = int.Parse(unitNode.Attributes["y"].Value);

    //                     pieces.Add(new Piece(pieceId, player, pieceType, posX, posY));
    //                 }
    //                 turns.Add(pieces);
    //             }

    //             // Agora você tem acesso aos dados do XML e pode usá-los para inicializar o estado do jogo
    //         }
    //     }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


