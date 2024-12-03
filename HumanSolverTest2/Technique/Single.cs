using Sudoku.Shared;

namespace HumanSolverTest2.Technique;
public class HiddenSingle
{
    public static bool Apply(SudokuGrid grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                // Skip filled cells
                if (grid.Cells[row, col] != 0) continue;

                var availableNumbers = grid.GetAvailableNumbers(row, col);
                
                foreach (int number in availableNumbers)
                {
                    if (IsHiddenSingle(grid, row, col, number))
                    {
                        grid.Cells[row, col] = number;
                        Console.WriteLine("Hidden single",
                            "{0}: {1}",
                            (row, col), number);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static bool IsHiddenSingle(SudokuGrid grid, int row, int col, int number)
    {
        // Check row
        bool isUniqueInRow = IsUniqueInGroup(grid, number, 
            SudokuGrid.CellNeighbours[row][col].Where(n => n.row == row));

        // Check column
        bool isUniqueInColumn = IsUniqueInGroup(grid, number, 
            SudokuGrid.CellNeighbours[row][col].Where(n => n.column == col));

        // Check box
        bool isUniqueInBox = IsUniqueInGroup(grid, number, 
            SudokuGrid.CellNeighbours[row][col].Where(n => 
                (n.row / 3 == row / 3) && (n.column / 3 == col / 3)));

        return isUniqueInRow || isUniqueInColumn || isUniqueInBox;
    }

    private static bool IsUniqueInGroup(SudokuGrid grid, int number, IEnumerable<(int row, int column)> group)
    {
        foreach (var cell in group)
        {
            if (grid.Cells[cell.row, cell.column] == 0)
            {
                var availableNumbers = grid.GetAvailableNumbers(cell.row, cell.column);
                if (availableNumbers.Contains(number))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public class NakedSingle
{
    public static bool Apply(SudokuGrid grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid.Cells[row, col] != 0) continue;

                var availableNumbers = grid.GetAvailableNumbers(row, col);
                if (availableNumbers.Length == 1)
                {
                    grid.Cells[row, col] = availableNumbers[0];
                    Console.WriteLine("Naked single",
                        "{0}: {1}",
                        (row, col), availableNumbers[0]);
                    return true;
                }
            }
        }
        return false;
    }
}