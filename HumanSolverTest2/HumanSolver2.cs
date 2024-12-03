using HumanSolverTest2.Technique;
using Sudoku.Shared;
using HiddenSingle = HumanSolverTest2.Technique.HiddenSingle;

namespace HumanSolverTest2;

public class HumanSolver2 : ISudokuSolver
{
    public SudokuGrid Solve(SudokuGrid s)
    {
        bool progress;
        do
        {
            progress = ApplyTechniques(s);

            // If no techniques work but the puzzle isn't solved, it’s unsolvable with logic
            if (!progress && !IsSolved(s))
            {
                throw new InvalidOperationException("This puzzle requires guessing or advanced techniques not supported by this solver.");
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
        if (NakedSingle.Apply(s)) return true;
        if (HiddenSingle.Apply(s)) return true;
        if (NakedPair.Apply(s)) return true;
        //if (HiddenPair.Apply(s)) return true;
        if (NakedTriplet.Apply(s)) return true;
        if (HiddenTriplet.Apply(s)) return true;
        if (PointingPairs.Apply(s)) return true;

        return false; // No progress made
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