import time
from multiprocessing import Process, Queue
from sudoku_cnn_solver import solve_sudoku


class SolverPresenter:
    """
    Classe qui présente un solveur de Sudoku et gère une limite de temps.
    """

    def __init__(self, solver):
        """
        Initialise la classe avec un solveur donné.
        :param solver: Fonction ou classe capable de résoudre un Sudoku.
        """
        self.solver = solver

    def __str__(self):
        """
        Retourne le nom du solveur utilisé.
        """
        return self.solver.__class__.__name__ if hasattr(self.solver, "__class__") else str(self.solver)

    def solve_with_time_limit(self, puzzle, max_duration):
        """
        Résout une grille de Sudoku avec une limite de temps.
        :param puzzle: Grille de Sudoku à résoudre (format numpy ou liste).
        :param max_duration: Temps maximum autorisé en secondes.
        :return: Grille résolue ou lève une exception si le temps est dépassé.
        """
        def solver_process(queue, puzzle):
            """Exécute le solveur et renvoie la solution dans une file."""
            solution = self.solver(puzzle)
            queue.put(solution)

        # Créer une file pour échanger les données entre processus
        queue = Queue()
        process = Process(target=solver_process, args=(queue, puzzle))
        process.start()

        # Attendre la fin du processus ou atteindre le timeout
        process.join(timeout=max_duration)

        if process.is_alive():
            process.terminate()
            raise TimeoutError(f"Solver {self} a dépassé la limite de temps de {max_duration} secondes.")

        if queue.empty():
            raise RuntimeError(f"Le solveur {self} n'a pas pu fournir de solution.")

        return queue.get()


# --------------------------
# Exemple d'utilisation
# --------------------------
if __name__ == "__main__":
    from sudoku_cnn_solver import build_cnn_model, generate_dummy_data

    # Exemple : Charger un modèle CNN
    print("Initialisation du modèle...")
    model = build_cnn_model()

    # Exemple de grille de Sudoku
    X, _ = generate_dummy_data(nb_samples=1)
    sample_puzzle = X[0]

    # Initialiser le SolverPresenter avec le solveur CNN
    solver_presenter = SolverPresenter(solver=lambda grid: solve_sudoku(model, grid))

    # Résoudre avec une limite de temps de 5 secondes
    try:
        print("Résolution de la grille avec limite de temps...")
        solved_grid = solver_presenter.solve_with_time_limit(sample_puzzle, max_duration=5)
        print("Grille résolue :\n", solved_grid)
    except TimeoutError as e:
        print("Erreur :", e)
    except Exception as e:
        print("Une erreur est survenue :", e)
