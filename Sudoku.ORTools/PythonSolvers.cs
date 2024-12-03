using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using Sudoku.Shared;
namespace Sudoku.ORTools
{
    public class OrToolsPythonSolver : PythonSolverBase
    {
        public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
        {
            //System.Diagnostics.Debugger.Break();
            //For some reason, the Benchmark runner won't manage to get the mutex whereas individual execution doesn't cause issues
            //using (Py.GIL())
            //{
            // create a Python scopes
            using (PyModule scope = Py.CreateScope())

            {    var strSudoku = string.Join("", s.Cells.Cast<int>().Select(c => c.ToString(CultureInfo.InvariantCulture)));


				// var strSudoku = s.Cells.SelectMany(row => row.Select(c => c.ToString(CultureInfo.InvariantCulture))).Aggregate("", (current, c) => current + c);

				// convert the Person object to a PyObject
				PyObject pySudoku = strSudoku.ToPython();
                // create a Python variable "person"
                scope.Set("instance", pySudoku);
				
				AddNumpyConverterScript(scope);
                // the person object may now be used in Python
                string code = Resources.PythonOrtoolsSolver_Py;
                
                // Console.WriteLine($"Script python:{code}");
                scope.Exec(code);
                var result = scope.Get("r");
                var managedResult = result.As<int[,]>();
                // var managedJaggedArray = managedResult.ToJaggedArray();
                // return new Shared.SudokuGrid() { Cells = managedJaggedArray };
                var managedJaggedArray = managedResult.ToJaggedArray(); // Assurez-vous que 'ToJaggedArray()' produit bien un 'int[][]'
                var managedMultiArray = ConvertToMultidimensionalArray(managedJaggedArray);
                return new Shared.SudokuGrid() { Cells = managedMultiArray };

        
            }
            //}

            
        }
        
        protected override void InitializePythonComponents()
        {
            InstallPipModule("ortools");
            base.InitializePythonComponents();
        }

        public static int[,] ConvertToMultidimensionalArray(int[][] jaggedArray)
{
    int rows = jaggedArray.Length;
    int cols = jaggedArray[0].Length;
    int[,] result = new int[rows, cols];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            result[i, j] = jaggedArray[i][j];
        }
    }

    return result;
}

    }





public class OrToolsPythonSolverLinear : PythonSolverBase
{
    public override Shared.SudokuGrid Solve(Shared.SudokuGrid s)
    {
        // Utilisation du GIL pour exécuter du code Python
        using (Py.GIL())
        {
            // Création d'une portée Python
            using (PyModule scope = Py.CreateScope())
            {
                // Transformation des cellules du Sudoku en une chaîne de caractères
                var strSudoku = string.Join("", s.Cells.Cast<int>().Select(c => c.ToString(CultureInfo.InvariantCulture)));

                // Conversion de la chaîne en objet Python
                PyObject pySudoku = strSudoku.ToPython();

                // Définir l'objet Python dans le scope
                scope.Set("instance", pySudoku);

                // Ajouter le script de conversion NumPy
                AddNumpyConverterScript(scope);

                // Code Python pour résoudre le Sudoku (utilisation de OR-Tools)
                string code = Resources.PythonOrtoolsSolverLinear_Py;
                scope.Exec(code);

                // Récupération du résultat Python sous forme de tableau
                var result = scope.Get("r");
                var managedResult = result.As<int[,]>();

                // Conversion du tableau multidimensionnel en tableau à jagged
                var managedJaggedArray = managedResult.ToJaggedArray();
                var managedMultiArray = ConvertToMultidimensionalArray(managedJaggedArray);

                // Création de la grille Sudoku résultante
                return new Shared.SudokuGrid() { Cells = managedMultiArray };
            }
        }
    }

    protected override void InitializePythonComponents()
    {
        // Installation du module Python OR-Tools
        InstallPipModule("ortools");
        base.InitializePythonComponents();
    }

    public static int[,] ConvertToMultidimensionalArray(int[][] jaggedArray)
    {
        int rows = jaggedArray.Length;
        int cols = jaggedArray[0].Length;
        int[,] result = new int[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                result[i, j] = jaggedArray[i][j];
            }
        }

        return result;
    }
}
}

    