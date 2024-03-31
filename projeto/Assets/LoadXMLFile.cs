using UnityEngine;
using UnityEditor;
using System.IO;

public class LoadXMLFile : MonoBehaviour
{
    public TextAsset xmlAsset; // Referência ao ficheiro XML carregado

    public void OpenFileExplorer()
    {
        // Abre o diálogo de seleção de ficheiros e obtém o caminho do ficheiro selecionado
        string filePath = EditorUtility.OpenFilePanel("Open XML File", "", "xml");

        if (!string.IsNullOrEmpty(filePath))
        {
            // Carrega o ficheiro XML
            xmlAsset = Resources.Load<TextAsset>(filePath);
            if (xmlAsset != null)
            {
                // Chama uma função para processar o XML
                ProcessXML(xmlAsset.text);
            }
            else
            {
                Debug.LogError("Ficheiro XML não encontrado: " + filePath);
            }
        }
    }

    // Método para processar o XML
    private void ProcessXML(string xmlContent)
    {
        // Aqui podes implementar a lógica para ler e processar o XML
        // Por exemplo, podes percorrer os nós XML, ler os atributos, etc.
        // Podes usar a API de XML do C# para isso.
    }
}
