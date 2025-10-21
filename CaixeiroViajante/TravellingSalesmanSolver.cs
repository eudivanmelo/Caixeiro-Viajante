namespace CaixeiroViajante;

public class TravellingSalesmanSolver(int[,] distanceMatrix)
{
    private readonly int[,] distanceMatrix = distanceMatrix;
    private readonly int numNodes = distanceMatrix.GetLength(0);
    
    public class SolverResult
    {
        public List<int> BestPath { get; set; } = new();
        public int BestDistance { get; set; }
        public long PermutationsChecked { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public SolverResult Solve(int startNode, int endNode, IProgress<string>? progress = null)
    {
        var result = new SolverResult();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            if (startNode < 0 || startNode >= numNodes)
            {
                result.Success = false;
                result.Message = "Nó inicial inválido";
                return result;
            }

            string logFile = $"caminhos_tsp_{startNode}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            using StreamWriter writer = new(logFile);
            writer.WriteLine($"=== Problema do Caixeiro Viajante - Partindo de {startNode} ===");
            writer.WriteLine($"Total de cidades: {numNodes}");
            writer.WriteLine($"Início: {DateTime.Now:HH:mm:ss}");
            writer.WriteLine();

            progress?.Report($"Explorando todos os caminhos partindo de {startNode} visitando todas as cidades e retornando...");

            int bestDistance = int.MaxValue;
            List<int> bestPath = [];
            long pathsExplored = 0;
            
            List<int> currentPath = [startNode];
            HashSet<int> visited = [startNode];

            void ExplorePaths(int currentNode, int currentDistance)
            {
                if (visited.Count == numNodes)
                {
                    int returnDistance = distanceMatrix[currentNode, startNode];
                    if (returnDistance > 0)
                    {
                        pathsExplored++;
                        int totalDistance = currentDistance + returnDistance;
                        
                        List<int> completePath = [.. currentPath, startNode];
                        string pathString = string.Join(" -> ", completePath);
                        writer.WriteLine($"Caminho {pathsExplored}: {pathString} | Distância: {totalDistance}");
                        
                        if (totalDistance < bestDistance)
                        {
                            bestDistance = totalDistance;
                            bestPath = completePath;
                        }

                        if (pathsExplored % 1000 == 0)
                        {
                            int permsPerSecond = (int)(pathsExplored / stopwatch.Elapsed.TotalSeconds);
                            progress?.Report($"Caminhos explorados: {pathsExplored:N0} | {permsPerSecond:N0} caminhos/s | Melhor: {(bestDistance == int.MaxValue ? "∞" : bestDistance.ToString())}");
                        }
                    }
                    return;
                }

                for (int nextNode = 0; nextNode < numNodes; nextNode++)
                {
                    if (visited.Contains(nextNode))
                        continue;

                    int distance = distanceMatrix[currentNode, nextNode];
                    if (distance == 0)
                        continue;

                    int newDistance = currentDistance + distance;

                    currentPath.Add(nextNode);
                    visited.Add(nextNode);

                    ExplorePaths(nextNode, newDistance);

                    currentPath.RemoveAt(currentPath.Count - 1);
                    visited.Remove(nextNode);
                }
            }

            ExplorePaths(startNode, 0);

            writer.WriteLine();
            writer.WriteLine("=== RESULTADO FINAL ===");
            writer.WriteLine($"Total de caminhos explorados: {pathsExplored:N0}");
            writer.WriteLine($"Melhor caminho: {string.Join(" → ", bestPath)}");
            writer.WriteLine($"Melhor distância: {bestDistance}");
            writer.WriteLine($"Tempo decorrido: {stopwatch.Elapsed.TotalSeconds:F2}s");
            writer.WriteLine($"Fim: {DateTime.Now:HH:mm:ss}");

            stopwatch.Stop();

            result.BestPath = bestPath;
            result.BestDistance = bestDistance;
            result.PermutationsChecked = pathsExplored;
            result.ElapsedTime = stopwatch.Elapsed;
            result.Success = bestDistance < int.MaxValue;
            result.Message = result.Success 
                ? $"Melhor caminho encontrado com distância {bestDistance}" 
                : "Nenhum caminho válido encontrado";

            progress?.Report($"Concluído! Tempo: {result.ElapsedTime.TotalSeconds:F2}s | Caminhos explorados: {pathsExplored:N0}");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.Success = false;
            result.Message = $"Erro: {ex.Message}";
            result.ElapsedTime = stopwatch.Elapsed;
        }

        return result;
    }


    public int GetDistance(int from, int to)
    {
        if (from < 0 || from >= numNodes || to < 0 || to >= numNodes)
            return -1;
        return distanceMatrix[from, to];
    }
}
