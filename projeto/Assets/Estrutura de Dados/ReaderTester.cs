using UnityEngine;
using System;

public class ReaderTester : MonoBehaviour
{
    private XMLReader xmlReader;
    public GameObject boardGameObject;
    void Start()
    {
        // Crie uma instância da classe XMLReader
        XMLReader xmlReader = new XMLReader();

        // Carregue e leia o arquivo XML
        Game game = xmlReader.LoadXMLToRead(@"C:\Users\maria\Desktop\xmlTestMundos.txt", boardGameObject);
        //Game game = xmlReader.LoadXMLToRead(@"C:\Users\inesc\OneDrive\Ambiente de trabalho\xmlTestMundos.txt", boardGameObject);

        
        // Exiba o nome do jogo
        Debug.Log("Nome do jogo: " + game.name);


        // Exiba detalhes dos papéis dos jogadores
        Debug.Log("Papéis dos jogadores:");
        foreach (Player role in game.roles)
        {
            Debug.Log(role.name);
        }

                // Exiba detalhes do tabuleiro
        Debug.Log("Detalhes do tabuleiro:");
        Debug.Log("Largura do tabuleiro: " + game.board.Width);
        Debug.Log("Altura do tabuleiro: " + game.board.Height);

        // Exiba detalhes dos turnos
        Debug.Log("Número de turnos: " + game.nTurns);
        for (int i = 0; i < game.turns.Count; i++)
        {
            Debug.Log("Turno " + (i + 1) + ":");
            Unit[] unitsInTurn = game.turns[i];
            foreach (Unit unit in unitsInTurn)
            {
                Debug.Log("Tipo: " + unit.piece + ", Ação: " + unit.action);
            }
        }
    }
}
