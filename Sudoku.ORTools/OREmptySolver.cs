using Sudoku.Shared;
using System;

namespace Sudoku.ORTools;
public class OREmptySolver : ISudokuSolver
{
  public SudokuGrid Solve(SudokuGrid s)
        {
            return s.CloneSudoku();
        }
        
}
