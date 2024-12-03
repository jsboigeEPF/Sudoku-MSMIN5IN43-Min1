from ortools.sat.python import cp_model
import numpy as np

#instance = "902005403100063025508407060026309001057010290090670530240530600705200304080041950"


def decode_sudoku(sample: str) -> np.matrix:
    return np.matrix([np.array(list(sample[i:i+9])).astype(np.integer) for i in range(0, len(sample), 9)])   
        
def solve_with_cp(grid: np.matrix) -> (np.matrix):
    assert grid.shape == (9,9)
    
    grid_size = 9
    region_size = 3 
    model = cp_model.CpModel() 
    x = {}
    for i in range(grid_size):
        for j in range(grid_size):
            if grid[i, j] != 0:
                x[i, j] = grid[i, j] 
            else:
                x[i, j] = model.NewIntVar(1, grid_size, 'x[{},{}]'.format(i,j) ) 
                
                
    for i in range(grid_size):
        model.AddAllDifferent([x[i, j] for j in range(grid_size)])
    for j in range(grid_size):
        model.AddAllDifferent([x[i, j] for i in range(grid_size)])
    for row_idx in range(0, grid_size, region_size):
        for col_idx in range(0, grid_size, region_size):
            model.AddAllDifferent([x[row_idx + i, j] for j in range(col_idx, (col_idx + region_size)) for i in range(region_size)])
    solver = cp_model.CpSolver() 
    status = solver.Solve(model) 
    result = np.zeros((grid_size, grid_size)).astype(np.int32)
    for i in range(grid_size):
        for j in range(grid_size):
            result[i,j] = int(solver.Value(x[i,j]))
    return result
solution = solve_with_cp(decode_sudoku(instance))
r=asNetArray(solution)
#print(r)
#on est dans le solver ortools
