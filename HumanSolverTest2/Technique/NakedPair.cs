using Sudoku.Shared;

namespace HumanSolverTest2.Technique;

public class NakedPair
{
    public static bool Apply(SudokuGrid grid)
    {
        bool madeProgress = false;

        // Check each row, column and box for naked pairs
        for (int i = 0; i < 9; i++)
        {
            madeProgress |= CheckGroupForNakedPairs(grid, HumanSolverTest2.HumanSolver2.GetRowCells(i));
            madeProgress |= CheckGroupForNakedPairs(grid, HumanSolverTest2.HumanSolver2.GetColumnCells(i));
            madeProgress |= CheckGroupForNakedPairs(grid, HumanSolverTest2.HumanSolver2.GetBoxCells(i));
        }

        return madeProgress;
    }

    private static bool CheckGroupForNakedPairs(SudokuGrid grid, List<(int row, int col)> group)
    {
        bool madeProgress = false;
        var candidatePairs = new Dictionary<(int row, int col), int[]>();

        // Find all cells with exactly 2 candidates
        foreach (var cell in group)
        {
            if (grid.Cells[cell.row, cell.col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(cell.row, cell.col);
                if (candidates.Length == 2)
                {
                    candidatePairs.Add(cell, candidates);
                }
            }
        }

        // Look for matching pairs
        foreach (var pair1 in candidatePairs)
        {
            foreach (var pair2 in candidatePairs)
            {
                if (pair1.Key != pair2.Key && 
                    pair1.Value.SequenceEqual(pair2.Value))
                {
                    // Found a naked pair - remove these numbers from other cells in the group
                    madeProgress |= RemoveCandidatesFromOtherCells(grid, group, 
                        new[] { pair1.Key, pair2.Key }, pair1.Value);
                }
            }
        }

        return madeProgress;
    }

    private static bool RemoveCandidatesFromOtherCells(
        SudokuGrid grid, 
        List<(int row, int col)> group, 
        (int row, int col)[] pairCells, 
        int[] numbersToRemove)
    {
        bool madeProgress = false;

        foreach (var cell in group)
        {
            if (!pairCells.Contains(cell) && grid.Cells[cell.row, cell.col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(cell.row, cell.col).ToList();
                bool modified = false;

                foreach (int num in numbersToRemove)
                {
                    if (candidates.Contains(num))
                    {
                        candidates.Remove(num);
                        modified = true;
                    }
                }

                if (modified && candidates.Count == 1)
                {
                    grid.Cells[cell.row, cell.col] = candidates[0];
                    Console.WriteLine("Naked pair",
                        "{0}: {1}",
                        cell, candidates[0]);
                    madeProgress = true;
                }
            }
        }

        return madeProgress;
    }
    
}

