using Python.Runtime;
using Sudoku.Shared;

namespace Solver.PythonSolverAimaCSP
{
    public class AimaCSPPythonSolver : PythonSolverBase
    {
        public override SudokuGrid Solve(SudokuGrid s)
        {
            using (PyModule scope = Py.CreateScope())
            {
                // Injectez le script Python pour le solveur CSP avec AIMA
                string pythonCode = @"
import numpy as np
from constraint import Problem

def sudoku_csp_solver(grid):
    problem = Problem()

    # Ajoutez les variables (81 cases) avec les domaines possibles (1-9)
    for row in range(9):
        for col in range(9):
            if grid[row][col] == 0:
                problem.addVariable((row, col), range(1, 10))
            else:
                problem.addVariable((row, col), [grid[row][col]])

    # Contraintes pour les lignes
    for row in range(9):
        problem.addConstraint(lambda *args: len(set(args)) == 9, [(row, col) for col in range(9)])

    # Contraintes pour les colonnes
    for col in range(9):
        problem.addConstraint(lambda *args: len(set(args)) == 9, [(row, col) for row in range(9)])

    # Contraintes pour les sous-grilles 3x3
    for block_row in range(3):
        for block_col in range(3):
            cells = [(block_row * 3 + i, block_col * 3 + j) for i in range(3) for j in range(3)]
            problem.addConstraint(lambda *args: len(set(args)) == 9, cells)

    # Résoudre le CSP
    solution = problem.getSolution()

    # Reconstruire la grille
    solved_grid = np.zeros((9, 9), dtype=int)
    for (row, col), value in solution.items():
        solved_grid[row][col] = value

    return solved_grid
";
                scope.Exec(pythonCode);

                // Convertissez la grille Sudoku en tableau NumPy
                AddNumpyConverterScript(scope);
                var pyCells = AsNumpyArray(s.Cells, scope);

                // Injectez la grille dans le scope Python
                scope.Set("grid", pyCells);

                // Appelez le solveur Python
                scope.Exec("result = sudoku_csp_solver(grid)");

                // Récupérez le résultat en NumPy et reconvertissez-le en tableau .NET
                PyObject pyResult = scope.Get("result");
                int[,] managedResult = AsManagedArray(scope, pyResult);

                // Retournez le Sudoku résolu
                return new SudokuGrid() { Cells = managedResult };
            }
        }

        protected override void InitializePythonComponents()
        {
            // Installez les modules nécessaires
            InstallPipModule("numpy");
            InstallPipModule("python-constraint");
            base.InitializePythonComponents();
        }
    }
}
