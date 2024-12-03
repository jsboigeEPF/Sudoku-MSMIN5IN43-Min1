using Sudoku.Shared;

namespace HumanSolverTest2.Technique;

public class NakedTriplet
{
    public static bool Apply(SudokuGrid grid)
    {
        bool madeProgress = false;

        for (int i = 0; i < 9; i++)
        {
            madeProgress |= CheckGroupForNakedTriplets(grid, HumanSolverTest2.HumanSolver2.GetRowCells(i));
            madeProgress |= CheckGroupForNakedTriplets(grid, HumanSolverTest2.HumanSolver2.GetColumnCells(i));
            madeProgress |= CheckGroupForNakedTriplets(grid, HumanSolverTest2.HumanSolver2.GetBoxCells(i));
        }
        
        return madeProgress;
    }

    private static bool CheckGroupForNakedTriplets(SudokuGrid grid, List<(int row, int col)> group)
    {
        bool madeProgress = false;
        var candidateCells = new Dictionary<(int row, int col), HashSet<int>>();

        // Find all cells with 2 or 3 candidates
        foreach (var cell in group)
        {
            if (grid.Cells[cell.row, cell.col] == 0)
            {
                var candidates = grid.GetAvailableNumbers(cell.row, cell.col);
                if (candidates.Length <= 3)
                {
                    candidateCells.Add(cell, new HashSet<int>(candidates));
                }
            }
        }

        // Look for triplets
        foreach (var cell1 in candidateCells)
        {
            foreach (var cell2 in candidateCells.Where(c => c.Key != cell1.Key))
            {
                foreach (var cell3 in candidateCells.Where(c => c.Key != cell1.Key && c.Key != cell2.Key))
                {
                    var unionCandidates = new HashSet<int>(cell1.Value);
                    unionCandidates.UnionWith(cell2.Value);
                    unionCandidates.UnionWith(cell3.Value);

                    if (unionCandidates.Count == 3)
                    {
                        // Found a naked triplet
                        var tripletCells = new[] { cell1.Key, cell2.Key, cell3.Key };
                        madeProgress |= RemoveCandidatesFromOtherCells(grid, group, tripletCells, unionCandidates.ToArray());
                    }
                }
            }
        }

        return madeProgress;
    }

    private static bool RemoveCandidatesFromOtherCells(
        SudokuGrid grid, 
        List<(int row, int col)> group, 
        (int row, int col)[] tripletCells, 
        int[] numbersToRemove)
    {
        bool madeProgress = false;

        foreach (var cell in group)
        {
            if (!tripletCells.Contains(cell) && grid.Cells[cell.row, cell.col] == 0)
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
                    Console.WriteLine("Naked triplet",
                        "{0}: {1}",
                        cell, candidates[0]);
                    madeProgress = true;
                }
            }
        }

        return madeProgress;
    }
}

public class HiddenTriplet
{
    public static bool Apply(SudokuGrid grid)
    {
        bool madeProgress = false;

        for (int i = 0; i < 9; i++)
        {
            madeProgress |= CheckGroupForHiddenTriplets(grid, HumanSolverTest2.HumanSolver2.GetRowCells(i));
            madeProgress |= CheckGroupForHiddenTriplets(grid, HumanSolverTest2.HumanSolver2.GetColumnCells(i));
            madeProgress |= CheckGroupForHiddenTriplets(grid, HumanSolverTest2.HumanSolver2.GetBoxCells(i));
        }

        return madeProgress;
    }

    private static bool CheckGroupForHiddenTriplets(SudokuGrid grid, List<(int row, int col)> group)
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

        // Look for triplets
        for (int n1 = 1; n1 <= 9; n1++)
        {
            if (!candidatesCount.ContainsKey(n1)) continue;
            
            for (int n2 = n1 + 1; n2 <= 9; n2++)
            {
                if (!candidatesCount.ContainsKey(n2)) continue;

                for (int n3 = n2 + 1; n3 <= 9; n3++)
                {
                    if (!candidatesCount.ContainsKey(n3)) continue;

                    var cells = new HashSet<(int row, int col)>();
                    cells.UnionWith(candidatesCount[n1]);
                    cells.UnionWith(candidatesCount[n2]);
                    cells.UnionWith(candidatesCount[n3]);

                    if (cells.Count == 3)
                    {
                        // Found a hidden triplet
                        foreach (var cell in cells)
                        {
                            var candidates = grid.GetAvailableNumbers(cell.row, cell.col).ToList();
                            if (candidates.Count > 3)
                            {
                                // Si nous avons exactement 4 candidats et que 3 d'entre eux sont nos triplets
                                if (candidates.Count == 4 && 
                                    candidates.Count(c => c == n1 || c == n2 || c == n3) == 3)
                                {
                                    // Placer le nombre restant dans la cellule
                                    int remainingNumber = candidates.First(n => n != n1 && n != n2 && n != n3);
                                    grid.Cells[cell.row, cell.col] = remainingNumber;
                                    Console.WriteLine("Hidden triplet",
                                        "{0}: {1}",
                                        cell, remainingNumber);
                                    madeProgress = true;
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
