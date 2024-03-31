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
            ProcessXML(xmlReader);
        }
        else
        {
            Debug.LogError("Ficheiro XML não encontrado: " + xmlFilePath);
        }
    }

    void ProcessXML(XmlReader xmlReader)
    {
        // Aqui podes implementar a lógica para ler e processar o XML
        // Por exemplo, podes percorrer os nós XML, ler os atributos, etc.
        // Podes usar xmlReader.Read(), xmlReader.GetAttribute(), etc.
        // para aceder aos elementos do XML.
    }
}
