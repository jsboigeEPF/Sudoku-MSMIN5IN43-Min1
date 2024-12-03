using Sudoku.Shared;

namespace HumanSolverTest2;

public class SolvingTechnique
{
    public string Name { get; set; }
    public Func<SudokuGrid, bool> Apply { get; set; }
    public int Difficulty { get; set; }
}

