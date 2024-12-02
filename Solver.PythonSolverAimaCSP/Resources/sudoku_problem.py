import search


class Sudoku:
    def __init__(self, sudoku_state, pos_vals=None):
        """
        Initializes an instance of a Sudoku.

        Keyword arguments:
        sudoku_state -- a 9 by 9 list of lists representing all of the values in a sudoku problem.
        The entries in the list of lists may either be the numbers 1 to 9 representing the puzzle values at
        those positions, or 0 representing an empty entry.

        pos_vals -- optional argument representing the possible values matrix. If none is provided then one will
        be created for the given sudoku state.
        """

        if pos_vals is None:
            self.possible_values = [[[i for i in range(1, 10)] for n in range(9)] for z in range(9)]
            self.sudoku_state = [[0 for x in range(9)] for y in range(9)]

            for r, row in enumerate(sudoku_state):
                for c, entry in enumerate(row):
                    if entry != 0:
                        self.set_element(r, c, entry)
        else:
            self.sudoku_state = sudoku_state
            self.possible_values = pos_vals

    def get_state(self):
        """
        Gets the current state of the sudoku as a copy of the list of lists.
        """
        state = []
        for row in self.sudoku_state:
            state.append(list(row))

        return state

    def get_possible_values_copy(self):
        """
        Gets a copy of the possible values matrix.
        """
        vals = []
        for row in self.possible_values:
            val_row = []
            for val_list in row:
                val_row.append(list(val_list))
            vals.append(val_row)
        return vals

    def deep_copy(self):
        """
        Returns a deep copy of this instance of a Sudoku.
        """
        return Sudoku(self.get_state(), pos_vals=self.get_possible_values_copy())

    def set_element(self, row, col, entry):
        """
        Sets the value at the given row and column values to the given entry value.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the row of the desired entry.
        col -- an integer from 0 to 8 representing the column of the desired entry.
        entry -- an integer from 1 to 9 representing the value to be inserted at the given position
        """
        if entry < 1 or entry > 9:
            raise ValueError("Entry in sudoku must be between 1 and 9.")
        elif row < 0 or row > 8:
            raise ValueError("Row value must be between 0 and 8")
        elif col < 0 or col > 8:
            raise ValueError("Column value must be between 0 and 8")
        elif len(self.get_possible_values(row, col)) == 1 and self.get_possible_values(row, col)[0] == entry:
            self.sudoku_state[row][col] = entry
        else:
            self.sudoku_state[row][col] = entry
            self.possible_values[row][col] = [entry]

            neighbors = self.get_neighbors(row, col)
            arcs = []
            for n in neighbors:
                arcs.append(((row, col), n))
            search.arc_consistency_3(self, arcs)

    def get_entry(self, row, col):
        """
        Gets the entry at the given row and column values from this sudoku problem.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the row of the desired entry.
        col -- an integer from 0 to 8 representing the column of the desired entry.
        """
        if row < 0 or row > 8:
            raise ValueError("Row value must be between 0 and 8")
        elif col < 0 or col > 8:
            raise ValueError("Column value must be between 0 and 8")
        else:
            return self.sudoku_state[row][col]

    def get_row(self, row):
        """
        Gets the desired row from this sudoku problem.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the desired row.
        """
        if row < 0 or row > 8:
            raise ValueError("Row value must be between 0 and 8")
        else:
            return self.sudoku_state[row]

    def get_col(self, col):
        """
        Gets the desired column from this sudoku problem.

        Keyword arguments:
        col -- an integer from 0 to 8 representing the desired column.
        """
        if col < 0 or col > 8:
            raise ValueError("Column value must be between 0 and 8")
        else:
            column = []
            for i, r in enumerate(self.sudoku_state):
                column.append(r[col])
            return column

    def get_square(self, square_row, square_col):
        """
        Gets the 3x3 sub-square of this sudoku problem.

        Keyword arguments:
        square_row -- an integer from 0 to 2 representing the row of the desired sub-square.
        square_col -- an integer from 0 to 2 representing the column of the desired sub-square.
        """
        if square_row < 0 or square_row > 2:
            raise ValueError("Square row value must be between 0 and 2")
        elif square_col < 0 or square_col > 2:
            raise ValueError("Square column value must be between 0 and 2")
        else:
            square = []
            initial_row_num = square_row * 3
            initial_col_num = square_col * 3

            for i in range(initial_row_num, initial_row_num + 3):
                square.append(self.sudoku_state[i][initial_col_num:initial_col_num + 3])

            return square

    def get_square_as_list(self, square_row, square_col):
        """
        Gets the 3x3 sub-square of this sudoku problem in the form of a single list.

        Keyword arguments:
        square_row -- an integer from 0 to 2 representing the row of the desired sub-square.
        square_col -- an integer from 0 to 2 representing the column of the desired sub-square.
        """
        square = self.get_square(square_row, square_col)
        square_list = []
        for row in square:
            for entry in row:
                square_list.append(entry)

        return square_list

    def is_complete(self):
        """
        Determines if the state of this Sudoku has no empty entries.
        """
        for row in self.sudoku_state:
            for entry in row:
                if entry == 0:
                    return False

        return True

    def is_valid(self):
        """
        Determines if the current state is not in violation of any of the constraints of a sudoku puzzle.
        """

        for i in range(0, 9):
            for n in range(1, 10):
                if self.get_row(i).count(n) > 1 or self.get_col(i).count(n) > 1:
                    return False

        for sr in range(0, 3):
            for sc in range(0, 3):
                for n in range(1, 10):
                    if self.get_square_as_list(sr, sc).count(n) > 1:
                        return False

        for r in range(9):
            for c in range(9):
                if len(self.get_possible_values(r, c)) == 0:
                    return False

        return True

    def print_sudoku(self):
        """
        Prints the state of this sudoku to the output.
        """
        sudoku_string = "\n"
        for r in range(0, 9):
            for c in range(0, 9):
                sudoku_string += str(self.get_entry(r, c)) + " "
            sudoku_string += "\n"
        print(sudoku_string)

    def get_possible_values(self, row, col):
        """
        Gets the possible values of the given position in the puzzle.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the row of the desired entry.
        col -- an integer from 0 to 8 representing the column of the desired entry.
        """
        return self.possible_values[row][col]

    def set_possible_values(self, row, col, vals):
        """
        Sets the list of possible values for the given position to the given list of values.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the row of the desired entry.
        col -- an integer from 0 to 8 representing the column of the desired entry.
        vals -- a list of ints from 1 to 9 with no repeated values representing the possible values of this position.
        """
        self.possible_values[row][col] = vals

    def get_neighbors(self, row, col):
        """
        Gets a list of tuples representing all of the positions that interact with the given position.

        Keyword arguments:
        row -- an integer from 0 to 8 representing the row of the desired entry.
        col -- an integer from 0 to 8 representing the column of the desired entry.
        """
        neighbors = []

        for i in range(9):
            if i != row:
                neighbors.append((i, col))
            if i != col:
                neighbors.append((row, i))

        sr = row // 3
        sc = col // 3

        for r in range(sr * 3, (sr * 3) + 3):
            for c in range(sc * 3, (sc * 3) + 3):
                if r != row or c != col:
                    pos = (r, c)
                    if pos not in neighbors:
                        neighbors.append(pos)

        return neighbors
