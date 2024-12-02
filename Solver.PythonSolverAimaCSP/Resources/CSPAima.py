import numpy as np
from constraint import Problem
from timeit import default_timer

def sudoku_csp_solver(grid):
    """
    Résout un Sudoku donné sous forme de grille 9x9 en utilisant CSP (Constraint Satisfaction Problems).
    """
    start = default_timer()  # Démarrer le chronomètre

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
        problem.addConstraint(lambda *args: len(set(args)) == len(args), [(row, col) for col in range(9)])

    # Contraintes pour les colonnes
    for col in range(9):
        problem.addConstraint(lambda *args: len(set(args)) == len(args), [(row, col) for row in range(9)])

    # Contraintes pour les sous-grilles 3x3
    for block_row in range(3):
        for block_col in range(3):
            cells = [(block_row * 3 + i, block_col * 3 + j) for i in range(3) for j in range(3)]
            problem.addConstraint(lambda *args: len(set(args)) == len(args), cells)

    # Résoudre le CSP
    solution = problem.getSolution()

    if not solution:
        return None, None  # Retourner `None` si aucune solution n'est trouvée

    # Reconstruire la grille à partir de la solution
    solved_grid = np.zeros((9, 9), dtype=int)
    for (row, col), value in solution.items():
        solved_grid[row][col] = value

    end = default_timer()  # Arrêter le chronomètre
    execution_time = end - start  # Temps d'exécution en secondes

    return solved_grid, execution_time


# Vérifiez si une grille `instance` existe déjà dans le scope (injection par Python.NET)
if 'instance' not in locals():
    # Exemple de grille par défaut si aucune n'est injectée
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

# Résoudre le Sudoku injecté ou par défaut
solved_grid, time_exec = sudoku_csp_solver(instance)

# Vérifiez si une solution a été trouvée
if solved_grid is not None:
    print('Sudoku résolu par CSP AIMA avec succès.')
    result = solved_grid  # Variable `result` à utiliser dans le code C#
    execution_time = time_exec  # Temps d'exécution à utiliser dans le code C#
else:
    print('Aucune solution trouvée.')
    result = None
    execution_time = None

# Affichez les résultats (utile pour debug)
if result is not None:
    print("Grille résolue :\n", result)
print(f"Temps de résolution : {execution_time * 1000 if execution_time else 'N/A'} ms")
