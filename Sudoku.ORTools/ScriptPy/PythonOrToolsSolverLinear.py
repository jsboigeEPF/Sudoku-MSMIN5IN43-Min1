from ortools.linear_solver import pywraplp
import numpy as np



#instance = "902005403100063025508407060026309001057010290090670530240530600705200304080041950"

def decode_sudoku(sample: str) -> np.matrix:
    return np.matrix([np.array(list(sample[i:i+9])).astype(np.integer) for i in range(0, len(sample), 9)])   
     


def solve_with_ip(grid: np.matrix) -> (np.matrix):
    
    assert grid.shape == (9,9)
    
    grid_size = 9
    cell_size = 3 
    solver = pywraplp.Solver('Sudoku Solver', pywraplp.Solver.CBC_MIXED_INTEGER_PROGRAMMING) 
    x = {}



    for i in range(grid_size):
        for j in range(grid_size):
            for k in range(grid_size):
                x[i, j, k] = solver.BoolVar('x[%i,%i,%i]' % (i, j, k))
    
    
   
    for i in range(grid_size):
        for j in range(grid_size):
            defined = grid[i, j] != 0
            if defined:
                solver.Add(x[i,j,grid[i, j]-1] == 1)
    
    

    for i in range(grid_size):
        for j in range(grid_size):
            solver.Add(solver.Sum([x[i, j, k] for k in range(grid_size)]) == 1)
   
    for k in range(grid_size):
        
        for i in range(grid_size):
            solver.Add(solver.Sum([x[i, j, k] for j in range(grid_size)]) == 1)

    
        for j in range(grid_size):
            solver.Add(solver.Sum([x[i, j, k] for i in range(grid_size)]) == 1)

       
        for row_idx in range(0, grid_size, cell_size):
            for col_idx in range(0, grid_size, cell_size):
                solver.Add(solver.Sum([x[row_idx + i, j, k] for j in range(col_idx, (col_idx + cell_size)) for i in range(cell_size)]) == 1)
   
   
    status = solver.Solve()
    statusdict = {0:'OPTIMAL', 1:'FEASIBLE', 2:'INFEASIBLE', 3:'UNBOUNDED', 
                  4:'ABNORMAL', 5:'MODEL_INVALID', 6:'NOT_SOLVED'}
    
    result = np.zeros((grid_size, grid_size)).astype(np.int32)
    if status == pywraplp.Solver.OPTIMAL:
        for i in range(grid_size):
            for j in range(grid_size):
                result[i,j] = sum((k + 1) * int(x[i, j, k].solution_value()) for k in range(grid_size))
    else:
        raise Exception('Unfeasible Sudoku: {}'.format(statusdict[status]))

    return result

solution = solve_with_ip(decode_sudoku(instance))
print(solution)
r=asNetArray(solution)