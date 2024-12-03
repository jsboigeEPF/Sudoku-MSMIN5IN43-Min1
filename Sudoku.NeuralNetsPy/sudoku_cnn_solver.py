import numpy as np
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import Conv2D, Flatten, Dense, Reshape
from tensorflow.keras.optimizers import Adam
import time


# --------------------------
# Générer ou charger les données
# --------------------------
def generate_dummy_data(nb_samples=1000):
    """Génère des grilles de Sudoku simplifiées pour l'entraînement et la validation."""
    X = np.zeros((nb_samples, 9, 9, 1))  # Sudoku partiellement rempli (entrée)
    y = np.zeros((nb_samples, 9, 9, 1))  # Sudoku complet (solution cible)
    for i in range(nb_samples):
        # Exemple simplifié : remplir avec des valeurs arbitraires
        X[i, :3, :3, 0] = np.random.randint(1, 10, (3, 3))
        y[i, :, :, 0] = np.random.randint(1, 10, (9, 9))
    return X, y


# --------------------------
# Modèle de réseau de neurones convolutif
# --------------------------
def build_cnn_model():
    """Construit un modèle CNN pour résoudre le Sudoku."""
    model = Sequential([
        Conv2D(64, (3, 3), activation='relu', padding='same', input_shape=(9, 9, 1)),
        Conv2D(128, (3, 3), activation='relu', padding='same'),
        Flatten(),
        Dense(256, activation='relu'),
        Dense(81, activation='softmax'),
        Reshape((9, 9, 1))  # Sortie en format Sudoku
    ])
    model.compile(optimizer=Adam(learning_rate=0.001), loss='sparse_categorical_crossentropy', metrics=['accuracy'])
    return model


# --------------------------
# Entraînement du modèle
# --------------------------
def train_model(model, X_train, y_train, epochs=10, batch_size=32):
    """Entraîne le modèle CNN avec les données générées."""
    history = model.fit(X_train, y_train, epochs=epochs, batch_size=batch_size, validation_split=0.2)
    return history


# --------------------------
# Résolution d'un Sudoku
# --------------------------
def solve_sudoku(model, sudoku_grid):
    """Résout une grille de Sudoku partielle avec le modèle entraîné."""
    sudoku_input = np.array(sudoku_grid).reshape((1, 9, 9, 1)) / 9.0  # Normalisation
    prediction = model.predict(sudoku_input)
    solved_sudoku = np.argmax(prediction.reshape((9, 9, 1)), axis=-1) + 1
    return solved_sudoku


# --------------------------
# Benchmark
# --------------------------
def benchmark_solver(model, test_data, max_duration=10):
    """Évalue le modèle sur des données de test."""
    start_time = time.time()
    for i, grid in enumerate(test_data):
        if time.time() - start_time > max_duration:
            print(f"Terminé après {i} grilles.")
            break
        solved = solve_sudoku(model, grid)
        print(f"Grille résolue :\n{solved}")


# --------------------------
# Main : Entraîner et résoudre
# --------------------------
if __name__ == "__main__":
    # Générer des données
    X, y = generate_dummy_data(nb_samples=1000)

    # Construire le modèle
    cnn_model = build_cnn_model()

    # Entraîner le modèle
    print("Entraînement du modèle...")
    train_model(cnn_model, X, y, epochs=5)

    # Benchmark avec données de test
    print("Benchmarking...")
    test_data = X[:10]  # Exemple avec 10 grilles de test
    benchmark_solver(cnn_model, test_data)
