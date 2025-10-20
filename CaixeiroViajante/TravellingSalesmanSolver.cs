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
            if (startNode < 0 || startNode >= numNodes || endNode < 0 || endNode >= numNodes)
            {
                result.Success = false;
                result.Message = "Nó inicial ou final inválido";
                return result;
            }

            if (startNode == endNode)
            {
                result.Success = false;
                result.Message = "Nó inicial e final devem ser diferentes";
                return result;
            }

            progress?.Report($"Iniciando cálculo de {startNode} até {endNode}...");

            List<int> intermediateNodes = [];
            for (int i = 0; i < numNodes; i++)
            {
                if (i != startNode && i != endNode)
                {
                    intermediateNodes.Add(i);
                }
            }

            int bestDistance = int.MaxValue;
            List<int> bestPath = [];
            long permutationsChecked = 0;

            long totalPermutations = 1;
            for (int size = 1; size <= intermediateNodes.Count; size++)
            {
                totalPermutations += Combinations(intermediateNodes.Count, size) * Factorial(size);
            }

            progress?.Report($"Total de caminhos a testar: {totalPermutations:N0}");

            int directDistance = distanceMatrix[startNode, endNode];
            if (directDistance > 0)
            {
                bestDistance = directDistance;
                bestPath = [startNode, endNode];
                progress?.Report($"Caminho direto: {startNode} → {endNode} = {directDistance}");
            }
            permutationsChecked++;

            for (int size = 1; size <= intermediateNodes.Count; size++)
            {
                var combinations = GetCombinations(intermediateNodes, size);

                foreach (var combination in combinations)
                {
                    foreach (var permutation in GetPermutations(combination.ToList()))
                    {
                        permutationsChecked++;

                        List<int> currentPath = [startNode, .. permutation, endNode];
                        int currentDistance = CalculatePathDistance(currentPath);

                        if (currentDistance > 0 && currentDistance < int.MaxValue && currentDistance < bestDistance)
                        {
                            bestDistance = currentDistance;
                            bestPath = [.. currentPath];
                        }

                        if (permutationsChecked % 1000 == 0)
                        {
                            double percentage = permutationsChecked * 100.0 / totalPermutations;
                            int permsPerSecond = (int)(permutationsChecked / stopwatch.Elapsed.TotalSeconds);
                            progress?.Report($"Progresso: {percentage:F2}% ({permutationsChecked:N0}/{totalPermutations:N0}) | {permsPerSecond:N0} perm/s | Melhor: {bestDistance}");
                        }
                    }
                }
            }

            stopwatch.Stop();

            result.BestPath = bestPath;
            result.BestDistance = bestDistance;
            result.PermutationsChecked = permutationsChecked;
            result.ElapsedTime = stopwatch.Elapsed;
            result.Success = bestDistance < int.MaxValue;
            result.Message = result.Success 
                ? $"Melhor caminho encontrado com distância {bestDistance}" 
                : "Nenhum caminho válido encontrado";

            progress?.Report($"Concluído! Tempo: {result.ElapsedTime.TotalSeconds:F2}s");
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

    private int CalculatePathDistance(List<int> path)
    {
        int totalDistance = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            int from = path[i];
            int to = path[i + 1];
            int distance = distanceMatrix[from, to];

            if (distance == 0)
            {
                return -1;
            }

            totalDistance += distance;
        }

        return totalDistance;
    }

    private static IEnumerable<List<int>> GetPermutations(List<int> list)
    {
        if (list.Count == 0)
        {
            yield return new List<int>();
            yield break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            int element = list[i];
            List<int> remaining = [.. list];
            remaining.RemoveAt(i);

            foreach (var permutation in GetPermutations(remaining))
            {
                List<int> result = [element, .. permutation];
                yield return result;
            }
        }
    }

    private static long Factorial(int n)
    {
        if (n <= 1) return 1;
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }

    private static long Combinations(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        
        long result = 1;
        for (int i = 1; i <= k; i++)
        {
            result = result * (n - i + 1) / i;
        }
        return result;
    }

    private static IEnumerable<List<int>> GetCombinations(List<int> list, int k)
    {
        if (k == 0)
        {
            yield return [];
            yield break;
        }

        if (k > list.Count)
        {
            yield break;
        }

        if (k == list.Count)
        {
            yield return [.. list];
            yield break;
        }

        int first = list[0];
        List<int> rest = list.GetRange(1, list.Count - 1);

        foreach (var combination in GetCombinations(rest, k - 1))
        {
            yield return [first, .. combination];
        }

        foreach (var combination in GetCombinations(rest, k))
        {
            yield return combination;
        }
    }

    public int GetDistance(int from, int to)
    {
        if (from < 0 || from >= numNodes || to < 0 || to >= numNodes)
            return -1;
        return distanceMatrix[from, to];
    }
}
