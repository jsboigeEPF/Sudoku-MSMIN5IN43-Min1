namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVRandomMAC : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv", "random", "mac");  // Combinaison MRV + random + MAC
        }
    }
}
