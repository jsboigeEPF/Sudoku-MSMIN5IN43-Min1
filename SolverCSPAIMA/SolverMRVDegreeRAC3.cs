namespace Sudoku.SolverCSPAIMA
{
    public class SolverMRVDegreeRAC3 : SolverCSPAIMA
    {
        protected override (string heuristic, string valueOrder, string inferenceMethod) GetStrategies()
        {
            return ("mrv_degree", "random", "ac3");  // Combinaison MRV + Degree + random + AC3
        }
    }
}
