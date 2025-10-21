namespace CaixeiroViajante;

/// <summary>
/// Versão console do Problema do Caixeiro Viajante
/// </summary>
static class ProgramConsole
{
    // Matriz de distâncias entre as cidades
    private static readonly int[,] coordsDistances = new int[,]
    {
        {0, 20, 0, 0, 0, 0, 0, 29, 0, 0, 0, 29, 37, 0, 0, 0, 0, 0},
        {20, 0, 25, 0, 0, 0, 0, 28, 0, 0, 0, 39, 0, 0, 0, 0, 0, 0},
        {0, 25, 0, 25, 0, 0, 0, 30, 0, 0, 0, 0, 54, 0, 0, 0, 0, 0},
        {0, 0, 25, 0, 39, 32, 42, 0, 23, 33, 0, 0, 0, 56, 0, 0, 0, 0},
        {0, 0, 0, 39, 0, 12, 26, 0, 0, 19, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 32, 12, 0, 17, 0, 0, 35, 30, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 42, 26, 17, 0, 0, 0, 0, 38, 0, 0, 0, 0, 0, 0, 0},
        {29, 28, 30, 0, 0, 0, 0, 0, 0, 0, 0, 25, 22, 0, 0, 0, 0, 0},
        {0, 0, 0, 23, 0, 0, 0, 0, 0, 26, 0, 0, 34, 0, 0, 0, 0, 0},
        {0, 0, 0, 33, 19, 35, 0, 0, 26, 0, 24, 0, 0, 30, 19, 0, 0, 0},
        {0, 0, 0, 0, 0, 30, 38, 0, 0, 24, 0, 0, 0, 0, 26, 0, 0, 36},
        {29, 39, 0, 0, 0, 0, 0, 25, 0, 0, 0, 0, 27, 0, 0, 43, 0, 0},
        {37, 0, 54, 0, 0, 0, 0, 22, 34, 0, 0, 27, 0, 24, 0, 19, 0, 0},
        {0, 0, 0, 56, 0, 0, 0, 0, 0, 30, 0, 0, 24, 0, 20, 19, 17, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 19, 26, 0, 0, 20, 0, 0, 18, 21},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 43, 0, 43, 19, 19, 0, 0, 26, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 18, 26, 0, 15},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 36, 0, 0, 0, 21, 0, 15, 0}
    };

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║     PROBLEMA DO CAIXEIRO VIAJANTE - Console       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════╝");
        Console.WriteLine();

        int numCidades = coordsDistances.GetLength(0);
        Console.WriteLine($"Total de cidades no grafo: {numCidades} (0 a {numCidades - 1})");
        Console.WriteLine();

        // Solicitar cidade inicial
        int startNode = -1;
        while (startNode < 0 || startNode >= numCidades)
        {
            Console.Write($"Digite a cidade inicial (1 a {numCidades}): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= 1 && value <= numCidades)
            {
                startNode = value - 1;
            }
            else
            {
                Console.WriteLine($"❌ Valor inválido! Digite um número entre 1 e {numCidades}.");
            }
        }

        Console.WriteLine();
        Console.WriteLine($"🚀 Iniciando busca partindo da cidade {startNode}...");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine();

        // Criar o solver
        var solver = new TravellingSalesmanSolver(coordsDistances);

        // Progress reporter para atualizar o console
        var progress = new Progress<string>(message =>
        {
            // Limpa a linha atual e exibe o progresso
            Console.Write($"\r{new string(' ', Console.WindowWidth - 1)}\r");
            Console.Write(message);
        });

        // Resolver o problema
        var result = solver.Solve(startNode, startNode, progress);

        Console.WriteLine(); // Nova linha após o progresso
        Console.WriteLine();
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        if (result.Success)
        {
            Console.WriteLine("✅ SOLUÇÃO ENCONTRADA!");
            Console.WriteLine();
            Console.WriteLine($"🛣️  Melhor caminho: {string.Join(" → ", result.BestPath.Select(n => n + 1))}");
            Console.WriteLine($"📏 Distância total: {result.BestDistance}");
            Console.WriteLine($"🔍 Caminhos explorados: {result.PermutationsChecked:N0}");
            Console.WriteLine($"⏱️  Tempo decorrido: {result.ElapsedTime.TotalSeconds:F2}s");
            Console.WriteLine($"⚡ Velocidade média: {(result.PermutationsChecked / result.ElapsedTime.TotalSeconds):N0} caminhos/s");
        }
        else
        {
            Console.WriteLine("❌ NENHUMA SOLUÇÃO ENCONTRADA");
            Console.WriteLine($"Mensagem: {result.Message}");
        }

        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine();
        Console.WriteLine($"📄 Os detalhes completos foram salvos no diretório atual.");
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para sair...");
        Console.ReadLine();
    }
}
