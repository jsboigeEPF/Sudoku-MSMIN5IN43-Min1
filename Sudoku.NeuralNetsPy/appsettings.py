import json
import os

class PythonConfiguration:
    """
    Classe pour gérer la configuration Python à partir du fichier JSON.
    """
    def __init__(self, install_path=None, python_directory_name=None, lib_file_name=None):
        self.install_path = install_path
        self.python_directory_name = python_directory_name
        self.lib_file_name = lib_file_name

    def __str__(self):
        return f"PythonConfiguration(InstallPath={self.install_path}, PythonDirectoryName={self.python_directory_name}, LibFileName={self.lib_file_name})"


class AppSettings:
    """
    Classe pour lire et gérer les paramètres du fichier appsettings.json.
    """
    def __init__(self, config_file="appsettings.json"):
        self.config_file = config_file
        self.config_data = self._load_config()

    def _load_config(self):
        """Charge la configuration du fichier JSON."""
        with open(self.config_file, "r") as f:
            return json.load(f)

    def get_python_config(self, os_name):
        """Retourne la configuration Python pour un OS donné (par exemple, 'OSX' ou 'Linux')."""
        try:
            python_config_data = self.config_data['PythonConfig'].get(os_name)
            if python_config_data:
                return PythonConfiguration(**python_config_data)
            else:
                raise ValueError(f"Configuration pour {os_name} non trouvée dans {self.config_file}")
        except KeyError:
            raise ValueError(f"Clé 'PythonConfig' introuvable dans {self.config_file}")


# Exemple d'utilisation
if __name__ == "__main__":
    # Charger la configuration
    settings = AppSettings()

    # Obtenez la configuration pour un système d'exploitation spécifique (par exemple, 'OSX' ou 'Linux')
    try:
        os_name = "OSX"  # Changez en 'Linux' ou 'OSX' selon le besoin
        python_config = settings.get_python_config(os_name)
        print(f"Configuration pour {os_name} : {python_config}")
    except Exception as e:
        print(f"Erreur lors du chargement de la configuration : {e}")
