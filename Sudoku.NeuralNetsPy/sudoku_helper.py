import os
from typing import List
from sudoku_shared import SudokuGrid  # Vous devrez peut-être ajuster cette importation

class SudokuHelper:
    """
    Aide à la gestion des grilles de Sudoku.
    """
    PUZZLES_FOLDER_NAME = "Puzzles"

    @staticmethod
    def get_sudokus(difficulty: str) -> List[SudokuGrid]:
        """
        Récupère les grilles de Sudoku en fonction de la difficulté.
        
        :param difficulty: Niveau de difficulté (Easy, Medium, Hard).
        :return: Liste d'objets SudokuGrid représentant les grilles de Sudoku.
        """
        file_name = {
            "Easy": "Sudoku_Easy51.txt",
            "Medium": "Sudoku_hardest.txt",
            "Hard": "Sudoku_top95.txt"
        }.get(difficulty, "Sudoku_top95.txt")  # Default to Hard if invalid difficulty
        
        puzzles_directory = None
        current_directory = os.getcwd()  # Répertoire courant

        # Chercher le dossier "Puzzles" en remontant dans l'arborescence des répertoires
        while current_directory != "/":
            subdirectories = os.listdir(current_directory)
            if SudokuHelper.PUZZLES_FOLDER_NAME in subdirectories:
                puzzles_directory = os.path.join(current_directory, SudokuHelper.PUZZLES_FOLDER_NAME)
                break
            current_directory = os.path.dirname(current_directory)
        
        if puzzles_directory is None:
            raise FileNotFoundError("Couldn't find puzzles directory")

        file_path = os.path.join(puzzles_directory, file_name)

        # Lire le fichier de Sudoku (il faudra peut-être adapter la lecture du fichier)
        sudokus = SudokuGrid.read_sudoku_file(file_path)
        return sudokus


# Exemple d'utilisation
if __name__ == "__main__":
    try:
        difficulty = "Easy"  # Peut être "Medium" ou "Hard"
        sudokus = SudokuHelper.get_sudokus(difficulty)
        for sudoku in sudokus:
            print(sudoku)  # Affiche chaque grille de Sudoku
    except Exception as e:
        print(f"Erreur : {e}")
