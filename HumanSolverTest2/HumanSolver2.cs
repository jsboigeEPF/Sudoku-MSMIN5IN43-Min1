using HumanSolverTest2.Technique;
using Sudoku.Shared;
using HiddenSingle = HumanSolverTest2.Technique.HiddenSingle;

namespace HumanSolverTest2;

public class HumanSolver2 : ISudokuSolver
{
    private readonly List<SolvingTechnique> _techniques;

    public HumanSolver2()
    {
        _techniques = new List<SolvingTechnique>
        {
            new SolvingTechnique { Name = "Naked Single", Apply = NakedSingle.Apply, Difficulty = 1 },
            new SolvingTechnique { Name = "Hidden Single", Apply = HiddenSingle.Apply, Difficulty = 2 },
            new SolvingTechnique { Name = "Naked Pair", Apply = NakedPair.Apply, Difficulty = 3 },
            new SolvingTechnique { Name = "Hidden Pair", Apply = HiddenPair.Apply, Difficulty = 4 },
            new SolvingTechnique { Name = "Hidden Rectangle", Apply = HiddenRectangle.Apply, Difficulty = 4 },
            new SolvingTechnique { Name = "Naked Triplet", Apply = NakedTriplet.Apply, Difficulty = 5 },
            new SolvingTechnique { Name = "Hidden Triplet", Apply = HiddenTriplet.Apply, Difficulty = 6 },
            new SolvingTechnique { Name = "Pointing Pairs", Apply = PointingPairs.Apply, Difficulty = 4 }
        };
    }

    public SudokuGrid Solve(SudokuGrid s)
    {
        bool progress;
        do
        {
            progress = ApplyTechniques(s);

            // If no techniques work but the puzzle isn't solved, it’s unsolvable with logic
            if (!progress && !IsSolved(s))
            {
                throw new InvalidOperationException(
                    "This puzzle requires guessing or advanced techniques not supported by this solver.");
            }
        } while (!IsSolved(s));

        return s;
    }

    /// <summary>
    /// Applies human solving techniques to the grid.
    /// </summary>
    /// <param name="s">The Sudoku grid.</param>
    /// <returns>True if progress was made, false otherwise.</returns>
    private bool ApplyTechniques(SudokuGrid s)
    {
        // Toujours essayer d'abord les techniques simples
        var simpleTechniques = _techniques.Where(t => t.Difficulty <= 2).ToList();
        foreach (var technique in simpleTechniques)
        {
            if (technique.Apply(s))
                return true;
        }

        // Si aucune technique simple ne fonctionne, essayer les techniques plus complexes
        var complexTechniques = _techniques.Where(t => t.Difficulty > 2).ToList();
        foreach (var technique in complexTechniques)
        {
            if (technique.Apply(s))
            {
                // Après une technique complexe, réessayer les techniques simples
                foreach (var simpleTechnique in simpleTechniques)
                {
                    if (simpleTechnique.Apply(s))
                        return true;
                }

                return true;
            }
        }

        return false;
    }

    private bool IsSolved(SudokuGrid s)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (s.Cells[row, col] == 0) return false; // Unsigned cell means it's unsolved
            }
        }

        return true;
    }

    public static List<(int row, int col)> GetRowCells(int rowIndex)
    {
        var cells = new List<(int row, int col)>();
        for (int col = 0; col < 9; col++)
        {
            cells.Add((rowIndex, col));
        }

        return cells;
    }

    public static List<(int row, int col)> GetColumnCells(int colIndex)
    {
        var cells = new List<(int row, int col)>();
        for (int row = 0; row < 9; row++)
        {
            cells.Add((row, colIndex));
        }

        return cells;
    }

    public static List<(int row, int col)> GetBoxCells(int boxIndex)
    {
        var cells = new List<(int row, int col)>();
        int startRow = (boxIndex / 3) * 3;
        int startCol = (boxIndex % 3) * 3;

        for (int row = startRow; row < startRow + 3; row++)
        {
            for (int col = startCol; col < startCol + 3; col++)
            {
                cells.Add((row, col));
            }
        }

        return cells;
    }
}