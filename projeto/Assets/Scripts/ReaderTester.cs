using System;

class ReaderTester
{
    static void Main(string[] args)
    {
        // Crie uma instância da classe XMLReader
        XMLReader xmlReader = new XMLReader();

        // Carregue e leia o arquivo XML
        //Game game = xmlReader.LoadXMLToRead(@"C:\Users\inesc\OneDrive\Ambiente de trabalho\xmlTestMundos.txt");
        Game game = null;

        Console.WriteLine("Nome do jogo: " + game.name);

        // Imprima detalhes do tabuleiro
        Console.WriteLine("Detalhes do tabuleiro:");
        Console.WriteLine("Largura do tabuleiro: " + game.board.Width);
        Console.WriteLine("Altura do tabuleiro: " + game.board.Height);

        // Imprima detalhes dos papéis dos jogadores
        Console.WriteLine("Papéis dos jogadores:");
        foreach (Player role in game.roles)
        {
            Console.WriteLine("- " + role.name);
        }

        // Imprima detalhes dos turnos
        Console.WriteLine("Número de turnos: " + game.nTurns);
        for (int i = 0; i < game.turns.Count; i++)
        {
            Console.WriteLine("Turno " + (i + 1) + ":");
            Unit[] unitsInTurn = game.turns[i];
            foreach (Unit unit in unitsInTurn)
            {
                Console.WriteLine("Tipo: " + unit.piece + ", Ação: " + unit.action);
            }
        }
    }
}
