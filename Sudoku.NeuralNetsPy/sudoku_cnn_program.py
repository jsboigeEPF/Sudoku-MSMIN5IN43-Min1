import time
import numpy as np
from tensorflow.keras.models import load_model
from sudoku_cnn_solver import build_cnn_model, generate_dummy_data, solve_sudoku


# --------------------------
# Gestion des grilles de Sudoku
# --------------------------
def load_sample_sudoku(difficulty="Easy"):
    """Charge des exemples de Sudoku pour différents niveaux de difficulté."""
    # Les grilles générées ici sont simplifiées. Utilisez un vrai dataset dans un projet complet.
    sudoku_easy = np.zeros((9, 9))
    sudoku_easy[:3, :3] = np.random.randint(1, 10, (3, 3))  # Exemple simple
    return sudoku_easy


# --------------------------
# Benchmark des Solveurs
# --------------------------
def benchmark_solver(model, test_data, max_duration=10):
    """Évalue le solveur CNN sur un ensemble de grilles."""
    print("Démarrage du benchmark...")
    start_time = time.time()
    for idx, grid in enumerate(test_data):
        if time.time() - start_time > max_duration:
            print(f"Terminé après {idx} grilles.")
            break
        solved = solve_sudoku(model, grid)
        print(f"Grille {idx + 1} résolue :\n{solved}")
    elapsed_time = time.time() - start_time
    print(f"Benchmark terminé en {elapsed_time:.2f} secondes.")


# --------------------------
# Menu Principal
# --------------------------
def run_menu():
    """Affiche le menu principal et gère les interactions."""
    while True:
        print("\nMenu Principal :")
        print("1 - Tester un solveur (Single Solver Test)")
        print("2 - Lancer un benchmark")
        print("3 - Quitter")

        choice = input("Choisissez une option : ")
        if choice == "1":
            single_solver_test()
        elif choice == "2":
            benchmark()
        elif choice == "3":
            print("Programme terminé.")
            break
        else:
            print("Choix invalide, veuillez réessayer.")


# --------------------------
# Test d'un Solveur Unique
# --------------------------
def single_solver_test():
    """Teste une seule grille avec le solveur CNN."""
    print("\n--- Test d'un Solveur ---")
    difficulty = input("Choisissez la difficulté (Easy, Medium, Hard) : ")
    sudoku = load_sample_sudoku(difficulty)
    print(f"Grille de Sudoku choisie ({difficulty}):\n{sudoku}")

    model = load_or_train_model()
    start_time = time.time()
    solved_sudoku = solve_sudoku(model, sudoku)
    elapsed = time.time() - start_time

    print(f"\nGrille résolue :\n{solved_sudoku}")
    print(f"Temps de résolution : {elapsed:.2f} secondes")


# --------------------------
# Benchmark
# --------------------------
def benchmark():
    """Lance un benchmark avec des grilles simulées."""
    print("\n--- Benchmark ---")
    model = load_or_train_model()
    difficulty = input("Choisissez la difficulté (Easy, Medium, Hard) : ")
    nb_grids = int(input("Nombre de grilles à résoudre : "))
    max_time = int(input("Temps maximum (secondes) : "))

    print(f"\nChargement des grilles ({difficulty})...")
    test_data, _ = generate_dummy_data(nb_samples=nb_grids)

    benchmark_solver(model, test_data, max_duration=max_time)


# --------------------------
# Charger ou entraîner le modèle
# --------------------------
def load_or_train_model():
    """Charge un modèle existant ou en entraîne un nouveau."""
    try:
        print("Chargement du modèle existant...")
        model = load_model("sudoku_cnn_model.h5")
    except IOError:
        print("Aucun modèle trouvé. Entraînement d'un nouveau modèle...")
        X, y = generate_dummy_data(nb_samples=1000)
        model = build_cnn_model()
        model.fit(X, y, epochs=5, batch_size=32, validation_split=0.2)
        model.save("sudoku_cnn_model.h5")
    return model


# --------------------------
# Programme Principal
# --------------------------
if __name__ == "__main__":
    run_menu()
