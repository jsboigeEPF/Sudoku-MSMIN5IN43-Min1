class PythonConfiguration:
    """
    Classe pour gérer la configuration Python.
    Utilisée pour stocker les chemins et fichiers nécessaires.
    """

    def __init__(self, install_path=None, python_directory_name=None, lib_file_name=None):
        self.install_path = install_path
        self.python_directory_name = python_directory_name
        self.lib_file_name = lib_file_name

    def load_from_dict(self, config_dict):
        """Charge la configuration à partir d'un dictionnaire."""
        self.install_path = config_dict.get("InstallPath")
        self.python_directory_name = config_dict.get("PythonDirectoryName")
        self.lib_file_name = config_dict.get("LibFileName")

    def to_dict(self):
        """Convertit la configuration en dictionnaire."""
        return {
            "InstallPath": self.install_path,
            "PythonDirectoryName": self.python_directory_name,
            "LibFileName": self.lib_file_name,
        }

    def __str__(self):
        """Affiche les informations de configuration."""
        return f"PythonConfiguration(InstallPath={self.install_path}, PythonDirectoryName={self.python_directory_name}, LibFileName={self.lib_file_name})"


# Exemple d'utilisation
if __name__ == "__main__":
    # Exemple de configuration
    config_dict = {
        "InstallPath": "/usr/local/python",
        "PythonDirectoryName": "python3.10",
        "LibFileName": "libpython3.10.so",
    }

    # Charger la configuration
    python_config = PythonConfiguration()
    python_config.load_from_dict(config_dict)

    # Afficher la configuration
    print(python_config)

    # Exporter en dictionnaire
    exported_dict = python_config.to_dict()
    print("Exported Configuration:", exported_dict)
