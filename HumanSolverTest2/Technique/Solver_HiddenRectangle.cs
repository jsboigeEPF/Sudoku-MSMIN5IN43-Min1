using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Shared;

public static class HiddenRectangle
{
public static bool Apply(SudokuGrid s)
{

	for (int x1 = 0; x1 < 9; x1++)
	{
		var c1 = s.GetColomn(x1);
		for (int x2 = x1 + 1; x2 < 9; x2++)
		{
			var c2 = s.GetColomn(x2);
			for (int y1 = 0; y1 < 9; y1++)
			{
				for (int y2 = y1 + 1; y2 < 9; y2++)
				{
					for (int value1 = 1; value1 <= 9; value1++)
					{
						for (int value2 = value1 + 1; value2 <= 9; value2++)
						{
							int[] candidates = [value1, value2];
							var cells = new []{ s.Cells[y1, x1], s.Cells[y2, x1],
								s.Cells[y1, x2], s.Cells[y2, x2]};
							if (cells.Any(c => !s.GetAvailableNumbers(x1, y1).Contains(value1)
							                   || !s.GetAvailableNumbers(x2, y2).Contains(value2)))
							{
								continue;
							}

							var groupedCells = cells.ToLookup(cell => s.GetAvailableNumbers(x1, y1).Count());
							var gtTwo = groupedCells.Where(g => g.Key > 2).SelectMany(g => g);
							int gtTwoCount = gtTwo.Count();
							if (gtTwoCount < 2 || gtTwoCount > 3)
							{
								continue;
							}

							bool changed = false;
							foreach (var c in groupedCells[2])
							{
								int newPointColomns = c == x1 ? x2 : x1;
								int newPointRow = c == y1 ? y2 : y1;
								foreach (int candidate in candidates)
								{
									if (!s.GetAvailableNumbers(newPointColomns, newPointRow).Contains(candidate))
									{
										s.Cells[newPointColomns, newPointRow] = candidate;
										changed = true;
									}
								}
							}

							if (changed)
							{
								
								Console.WriteLine("Hidden rectangle",
										"{0}: {1}",
										candidates, cells);
								return true;
							}
						}
					}
				}
			}
		}
	}

	return false;
}
}
