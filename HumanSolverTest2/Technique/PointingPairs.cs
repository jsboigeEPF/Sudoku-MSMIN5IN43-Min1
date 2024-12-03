using Sudoku.Shared;

namespace HumanSolverTest2.Technique;

public class PointingPairs
{
    public static bool Apply(SudokuGrid grid)
    {
        bool madeProgress = false;

        // Check each box
        for (int box = 0; box < 9; box++)
        {
            // For each number 1-9
            for (int num = 1; num <= 9; num++)
            {
                madeProgress |= CheckBoxForPointingPair(grid, box, num);
            }
        }

        return madeProgress;
    }

    private static bool CheckBoxForPointingPair(SudokuGrid grid, int box, int num)
    {
        bool madeProgress = false;
        int startRow = (box / 3) * 3;
        int startCol = (box % 3) * 3;

        var candidateCells = new List<(int row, int col)>();

        // Find all cells in the box where num is a candidate
        for (int r = startRow; r < startRow + 3; r++)
        {
            for (int c = startCol; c < startCol + 3; c++)
            {
                if (grid.Cells[r, c] == 0 && grid.GetAvailableNumbers(r, c).Contains(num))
                {
                    candidateCells.Add((r, c));
                }
            }
        }

        if (candidateCells.Count >= 2 && candidateCells.Count <= 3)
        {
            // Check if all candidates are in the same row
            if (candidateCells.All(cell => cell.row == candidateCells[0].row))
            {
                // Remove num from other cells in the same row but outside the box
                madeProgress |= RemoveFromRowExceptBox(grid, candidateCells[0].row, startCol, num);
            }
            // Check if all candidates are in the same column
            else if (candidateCells.All(cell => cell.col == candidateCells[0].col))
            {
                // Remove num from other cells in the same column but outside the box
                madeProgress |= RemoveFromColumnExceptBox(grid, candidateCells[0].col, startRow, num);
            }
        }

        return madeProgress;
    }

    private static bool RemoveFromRowExceptBox(SudokuGrid grid, int row, int startCol, int num)
    {
        bool madeProgress = false;
        int boxEndCol = startCol + 2;

        for (int col = 0; col < 9; col++)
        {
            if (col >= startCol && col <= boxEndCol) continue;

            if (grid.Cells[row, col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(row, col).ToList();
                if (candidates.Contains(num))
                {
                    candidates.Remove(num);
                    if (candidates.Count == 1)
                    {
                        grid.Cells[row, col] = candidates[0];
                        Console.WriteLine("Pointing pair",
                            "{0}: {1}",
                            (row, col), candidates[0]);
                        madeProgress = true;
                    }
                }
            }
        }

        return madeProgress;
    }

    private static bool RemoveFromColumnExceptBox(SudokuGrid grid, int col, int startRow, int num)
    {
        bool madeProgress = false;
        int boxEndRow = startRow + 2;

        for (int row = 0; row < 9; row++)
        {
            if (row >= startRow && row <= boxEndRow) continue;

            if (grid.Cells[row, col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(row, col).ToList();
                if (candidates.Contains(num))
                {
                    candidates.Remove(num);
                    if (candidates.Count == 1)
                    {
                        grid.Cells[row, col] = candidates[0];
                        Console.WriteLine("Pointing pair",
                            "{0}: {1}",
                            (row, col), candidates[0]);
                        madeProgress = true;
                    }
                }
            }
        }

        return madeProgress;
    }
}