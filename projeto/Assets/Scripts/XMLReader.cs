using UnityEngine;
using System.Xml;

public class XMLReader : MonoBehaviour
{
   // public string xmlFilePath; // Caminho do ficheiro XML

    public void LoadXMLToRead(string xmlFilePath)
    {
        // Carrega o ficheiro XML da pasta Resources (mudar isto)
        TextAsset xmlAsset = Resources.Load<TextAsset>(xmlFilePath);

        if (xmlAsset != null)
        {
            // Cria um novo leitor XML
            XmlReader xmlReader = XmlReader.Create(new System.IO.StringReader(xmlAsset.text));

            // Chama uma função para processar o XML
            ReadXML(xmlReader);
        }
        else
        {
            Debug.LogError("Ficheiro XML não encontrado: " + xmlFilePath);
        }
    }

    void ReadXML(XmlReader xmlReader)
    {
        //coisas para checkar DTD

        while(xmlReader.Read()){
            //switch case para escolher o que fazer com cada tag?? Se calhar ter uma função para cada tipo de tag, senão aqui fica too much
            //todos eles vão ter de entrar dentro dos seus child -> pôr mais um loop nessas funções para percorrer filhos?

            if (xmlReader.NodeType == XmlNodeType.Element){
                switch (xmlReader.Name){
                    case "game": 
                        string game_name = DealWithGame(xmlReader, game) break;

                    case "roles": 
                        Player[] roles = DealWithRoles(xmlReader, game) break;
                    
                    case "board": 
                        Board board = DealWithBoard(xmlReader, game) break;
                    
                    case "turns": 
                        Unit[][] turns = DealWithTurns(xmlReader, game) break;
                }
            }

        }
        Game game = new Game(board, roles, ??, game_name); 
    }

    string DealWithGame(XMLReader xmlr){
        return xmlr.GetAttribute("name");
    }

    Player[] DealWithRoles(XMLReader xmlr){
        List<Player> rolesList = new List<Player>();
        while (xmlReader.Read()){
            string p_name = xmlr.GetAttribute("name");
            Player p = new Player(p_name);
            rolesList.Add(player); 
        }
        return Player[] rolesArray = rolesList.ToArray();
    }

    void DealWithBoard(XMLReader xmlr){
        
    }

    void DealWithTurns(XMLReader xmlr){
        //se calhar aqui é o único que faz sentido ter um "loop" para ler logo uma turn, as units todas
    }

}
