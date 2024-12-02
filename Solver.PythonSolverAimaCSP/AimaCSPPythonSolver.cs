using System.Runtime.Versioning;
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
                Console.WriteLine("Before String");
                string pythonCode =Resources.CSPAima_py;
                Console.WriteLine(pythonCode);
                //Injectez le script en conversion
                // Convertissez la grille Sudoku en tableau NumPy
                AddNumpyConverterScript(scope);
                Console.WriteLine("After AddNumpyConverter");
                var pyCells = AsNumpyArray(s.Cells, scope);
                // Injectez la grille dans le scope Python
                Console.WriteLine("After AsNumpyArray");
                scope.Set("instance", pyCells);
                Console.WriteLine("After scopeSet");
                scope.Exec(pythonCode);
                Console.WriteLine("After scopExec");
                
                // Récupérez le résultat et le temps d'exécution
                PyObject pyResult = scope.Get("result");
                PyObject pyExecutionTime = scope.Get("execution_time");

                // Convertissez la grille résolue et le temps d'exécution
                int[,] managedResult = AsManagedArray(scope, pyResult);
                double executionTime = pyExecutionTime.As<double>();

                // Affichez le temps d'exécution dans la console (optionnel)
                Console.WriteLine($"Temps d'exécution : {executionTime * 1000} ms");

                // Retournez le Sudoku résolu
                return new SudokuGrid() { Cells = managedResult };
            }
        }

        protected override void InitializePythonComponents()
        {
            // Installez les modules nécessaires
            Console.WriteLine("install numpy");
            InstallPipModule("numpy");
            Console.WriteLine("install python constraint");
            InstallPipModule("python-constraint");
            base.InitializePythonComponents();
        }
    }
}
