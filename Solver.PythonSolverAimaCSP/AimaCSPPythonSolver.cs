using System;
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
                // Ajoutez le script de conversion entre .NET et NumPy
                AddNumpyConverterScript(scope);

                // Convertissez la grille Sudoku .NET en tableau NumPy
                var pyCells = AsNumpyArray(s.Cells, scope);

                // Injectez la grille Sudoku initiale dans le scope Python
                scope.Set("instance", pyCells);

                // Script Python inspiré de votre solution CSP
                string pythonCode = @"
import numpy as np
from collections import defaultdict
from queue import Queue
from timeit import default_timer  # Pour mesurer le temps de résolution


class SudokuCSP:
    def __init__(self, grid):
        self.grid = grid
        self.size = 9
        self.subgrid_size = 3

        # Variables : chaque cellule (row, col)
        self.variables = [(row, col) for row in range(self.size) for col in range(self.size)]

        # Domaines : chaque cellule a un domaine initial de 1-9 (ou une valeur fixe si préremplie)
        self.domains = {
            (row, col): [grid[row][col]] if grid[row][col] != 0 else list(range(1, 10))
            for row in range(self.size) for col in range(self.size)
        }

        # Contraintes : dictionnaire où chaque variable a une liste de voisins
        self.constraints = self.build_constraints()

    def build_constraints(self):
        '''Construit les contraintes pour chaque variable.'''
        constraints = defaultdict(list)
        for row in range(self.size):
            for col in range(self.size):
                neighbors = set()

                # Même ligne
                neighbors.update((row, c) for c in range(self.size) if c != col)

                # Même colonne
                neighbors.update((r, col) for r in range(self.size) if r != row)

                # Même sous-grille
                start_row, start_col = 3 * (row // 3), 3 * (col // 3)
                neighbors.update(
                    (r, c)
                    for r in range(start_row, start_row + 3)
                    for c in range(start_col, start_col + 3)
                    if (r, c) != (row, col)
                )

                constraints[(row, col)] = list(neighbors)
        return constraints

    def is_consistent(self, var, value, assignment):
        '''Vérifie si l'affectation est cohérente avec les contraintes.'''
        for neighbor in self.constraints[var]:
            if neighbor in assignment and assignment[neighbor] == value:
                return False
        return True

    def ac3(self):
        '''Applique l'algorithme AC-3 pour réduire les domaines.'''
        queue = Queue()
        for var in self.variables:
            for neighbor in self.constraints[var]:
                queue.put((var, neighbor))

        while not queue.empty():
            (var1, var2) = queue.get()
            if self.revise(var1, var2):
                if not self.domains[var1]:
                    return False
                for neighbor in self.constraints[var1]:
                    if neighbor != var2:
                        queue.put((neighbor, var1))
        return True

    def revise(self, var1, var2):
        '''Révise le domaine de var1 pour qu'il respecte les contraintes avec var2.'''
        revised = False
        for value in self.domains[var1][:]:
            if not any(self.is_consistent(var1, value, {var2: val}) for val in self.domains[var2]):
                self.domains[var1].remove(value)
                revised = True
        return revised

    def select_unassigned_variable(self, assignment):
        '''Heuristique MRV : choisir la variable avec le plus petit domaine non affecté.'''
        unassigned = [v for v in self.variables if v not in assignment]
        return min(unassigned, key=lambda var: len(self.domains[var]))

    def backtrack(self, assignment):
        '''Algorithme de backtracking avec heuristiques.'''
        if len(assignment) == len(self.variables):
            return assignment

        var = self.select_unassigned_variable(assignment)
        for value in self.domains[var]:
            if self.is_consistent(var, value, assignment):
                assignment[var] = value
                result = self.backtrack(assignment)
                if result:
                    return result
                del assignment[var]
        return None

    def solve(self):
        '''Résout le Sudoku en appliquant AC-3 et le backtracking.'''
        if not self.ac3():
            return None
        assignment = {}
        result = self.backtrack(assignment)
        if result:
            # Reconstruire la grille à partir de l'affectation
            for (row, col), value in result.items():
                self.grid[row][col] = value
            return self.grid
        return None


# Définir `instance` uniquement si non déjà défini par PythonNET
if 'instance' not in locals():
    instance = np.array([
        [0, 0, 0, 0, 9, 4, 0, 3, 0],
        [0, 0, 0, 5, 1, 0, 0, 0, 7],
        [0, 8, 9, 0, 0, 0, 0, 4, 0],
        [0, 0, 0, 0, 0, 0, 2, 0, 8],
        [0, 6, 0, 2, 0, 1, 0, 5, 0],
        [1, 0, 2, 0, 0, 0, 0, 0, 0],
        [0, 7, 0, 0, 0, 0, 5, 2, 0],
        [9, 0, 0, 0, 6, 5, 0, 0, 0],
        [0, 4, 0, 9, 7, 0, 0, 0, 0]
    ], dtype=int)

# Calcul du temps de résolution
start = default_timer()
solver = SudokuCSP(instance)
solved_grid = solver.solve()
result_time = default_timer() - start  # Temps en secondes

if solved_grid is not None:
    result = solved_grid  # `result` sera utilisé pour la récupération depuis C#
else:
    print('Aucune solution trouvée.')

# Affichage du temps de résolution
print('Le temps de résolution est de :', result_time * 1000, 'ms')

";
                // Exécutez le script Python dans le scope
                scope.Exec(pythonCode);

                // Récupérez la solution Python
                PyObject pyResult = scope.Get("result");

                // Vérifiez si la solution est valide
                if (pyResult == null)
                {
                    throw new Exception("Aucune solution trouvée pour la grille Sudoku.");
                }

                // Convertissez le tableau NumPy Python en tableau .NET
                int[,] managedResult = AsManagedArray(scope, pyResult);

                // Retournez la grille Sudoku résolue
                return new SudokuGrid() { Cells = managedResult };
            }
        }

        protected override void InitializePythonComponents()
        {
            // Installez les modules nécessaires pour la solution CSP
            InstallPipModule("numpy");
            base.InitializePythonComponents();
        }
    }
}
