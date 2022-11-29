using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle generation of path.
/// Ripped of from <see href="https://bitbucket.org/Sniffle6/tilemaps-with-astar/src/master/Assets/Astar.cs"/> though I would say that I have done favour to world by burning most of it.
/// </summary>
public class Astar {

    /// <summary>
    /// Max length for the path to be found, there shouldn't ever be path this long. 
    /// </summary>
    private const int maxLength = 300;

    /// <summary>
    /// List of spots reserved for path and path padding.
    /// </summary>
    private List<Spot> reservedTiles = new List<Spot>();

    /// <summary>
    /// Spots that need to be assessed.
    /// </summary>
    private List<Spot> openSet;

    /// <summary>
    /// Spots already been assessed.
    /// </summary>
    private List<Spot> closedSet;

    /// <summary>
    /// Every spot in grid.
    /// </summary>
    private Spot[,] grid;

    /// <summary>
    /// Amount of rows in grid.
    /// </summary>
    private int rows { get => grid.GetUpperBound(0) + 1; }

    /// <summary>
    /// Amount of columns in grid. 
    /// </summary>
    private int columns { get => grid.GetUpperBound(1) + 1; }


    /// <summary>
    /// Tries to find path from start to end, returns the shortest one if path found.
    /// </summary>
    /// <param name="tileData">height data of the enviroment.</param>
    /// <param name="start">Coordinate for path to start.</param>
    /// <param name="end">Coordinate for path to end.</param>
    /// <returns>Ascending list of spots from start to end of the shortest path | <see langword="null"/> if path not found</returns>
    public List<Spot> TryGeneratePath(int[,] tileData, Vector2Int start, Vector2Int end) {
        // Initialise and fill grid
        grid = new Spot[tileData.GetUpperBound(0) + 1, tileData.GetUpperBound(1) + 1];
        for (int x = 0; x < rows; x++)
            for (int y = 0; y < columns; y++)
                grid[x, y] = new Spot(x, y);

        // Initialize spots
        for (int x = 0; x < rows; x++)
            for (int y = 0; y < columns; y++)
                grid[x, y].Initialize(tileData[x, y], grid, grid[end.x, end.y]);

        // Initialize sets
        openSet = new List<Spot>() { grid[start.x, start.y] };
        closedSet = reservedTiles;

        // Loop trough all spots able to reach
        while (openSet.Count > 0) {
            // Get next spot, one with smallest cost in openSet
            Spot current = Next();
            // Check if end reached
            if (current == grid[end.x, end.y]) return Path(current);

            foreach (Spot neighbour in current.neighbours) {
                // If neighbour in closed set or is wall, ignore it
                if (closedSet.Any(_Spot => Spot.AreSame(_Spot, neighbour)) || neighbour.height != 0) continue;

                // If not in open set, add to it
                if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                // Ignore if in open set and has lower length
                else if (current.length > neighbour.length) continue;

                // This is new path for for neighbour
                neighbour.previous = current;
            }
        }

        // Path not found
        reservedTiles.Clear();
        return null;
    }


    /// <summary>
    /// Get next spot from <see cref="openSet"/> and move it to <see cref="closedSet"/>.
    /// </summary>
    /// <returns>Spot from <see cref="openSet"/> with lowest cost.</returns>
    private Spot Next() {
        Spot next = openSet.Aggregate((seed, value) => value.cost < seed.cost || value.cost == seed.cost && value.heuristic < seed.heuristic ? value : seed);

        openSet.Remove(next);
        closedSet.Add(next);

        return next;
    }



    /// <summary>
    /// Get path from start to end.
    /// </summary>
    /// <param name="temp">End</param>
    /// <returns></returns>
    private List<Spot> Path(Spot temp) {
        List<Spot> Path = new List<Spot>() { temp };

        while (temp.previous != null) {
            Path.Add(temp.previous);
            reservedTiles.AddRange(temp.previous.neighbours);
            temp = temp.previous;
        }

        if (maxLength - (Path.Count - 1) < 0) {
            Path.RemoveRange(0, (Path.Count - 1) - maxLength);
        }

        Path.Reverse();
        return Path;
    }
}