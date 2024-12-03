using Sudoku.Shared;


namespace HumanSolverTest2.Technique;

public class HiddenPair
{
    public static bool Apply(SudokuGrid grid)
    {
        bool madeProgress = false;

        // Check each row, column and box for hidden pairs
        for (int i = 0; i < 9; i++)
        {
            madeProgress |= CheckGroupForHiddenPairs(grid, HumanSolverTest2.HumanSolver2.GetRowCells(i));
            madeProgress |= CheckGroupForHiddenPairs(grid, HumanSolverTest2.HumanSolver2.GetColumnCells(i));
            madeProgress |= CheckGroupForHiddenPairs(grid, HumanSolverTest2.HumanSolver2.GetBoxCells(i));
        }

        return madeProgress;
    }

    private static bool CheckGroupForHiddenPairs(SudokuGrid grid, List<(int row, int col)> group)
    {
        bool madeProgress = false;
        var candidatesCount = new Dictionary<int, List<(int row, int col)>>();

        // Count occurrences of each candidate in the group
        foreach (var cell in group)
        {
            if (grid.Cells[cell.row, cell.col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(cell.row, cell.col);
                foreach (int candidate in candidates)
                {
                    if (!candidatesCount.ContainsKey(candidate))
                    {
                        candidatesCount[candidate] = new List<(int row, int col)>();
                    }
                    candidatesCount[candidate].Add(cell);
                }
            }
        }

        // Look for pairs of numbers that appear in exactly the same two cells
        for (int n1 = 1; n1 <= 9; n1++)
        {
            if (!candidatesCount.ContainsKey(n1)) continue;
            
            for (int n2 = n1 + 1; n2 <= 9; n2++)
            {
                if (!candidatesCount.ContainsKey(n2)) continue;

                var cells1 = candidatesCount[n1];
                var cells2 = candidatesCount[n2];

                if (cells1.Count == 2 && cells2.Count == 2 && 
                    cells1.SequenceEqual(cells2))
                {
                    // Found a hidden pair
                    foreach (var cell in cells1)
                    {
                        var candidates = grid.GetAvailableNumbers(cell.row, cell.col).ToList();
                        if (candidates.Count > 2)
                        {
                            // Si nous n'avons que deux candidats possibles, placer la valeur
                            if (candidates.Count(c => c == n1 || c == n2) == 2 && 
                                candidates.Count(c => c != n1 && c != n2) > 0)
                            {
                                madeProgress = true;
                                if (candidates.Count == 3)  // Si après élimination il ne reste qu'un candidat
                                {
                                    int remainingNumber = candidates.First(n => n != n1 && n != n2);
                                    grid.Cells[cell.row, cell.col] = remainingNumber;
                                    Console.WriteLine("Hidden pair",
                                        "{0}: {1}",
                                        (cell.row, cell.col), remainingNumber);
                                }
                            }
                        }
                    }
                }
            }
        }

        return madeProgress;
    }
}
