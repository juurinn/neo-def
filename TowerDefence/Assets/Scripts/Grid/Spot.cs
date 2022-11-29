using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Store data of spot.
/// </summary>
public class Spot {

    /// <summary>
    /// X coordinate of spot. 
    /// </summary>
    public int x { get; private set; }

    /// <summary>
    /// Y coordinate of spot. 
    /// </summary>
    public int y { get; private set; }

    /// <summary>
    /// Height of spot.
    /// </summary>
    public int height { get; private set; }

    /// <summary>
    /// Length of shortest path to get from start to this spot. 
    /// </summary>
    public int length { get; private set; }

    /// <summary>
    /// Heuristic of cost to get from this spot to end.
    /// </summary>
    public int heuristic { get; private set; }

    /// <summary>
    /// Cost of this spot. For comparing between other spots.
    /// </summary>
    public int cost { get => length + heuristic; }

    /// <summary>
    /// Neighbour spots of this spot.
    /// </summary>
    public List<Spot> neighbours { get; private set; }

    /// <summary>
    /// Last spot of shortest path to get to this spot.
    /// </summary>
    public Spot previous {
        get => Previous;
        set {
            length = value.length + 1;
            Previous = value;
        }
    }
    private Spot Previous = null;


    public Spot(int x, int y) {
        length = 0;
        this.x = x;
        this.y = y;
    }


    /// <summary>
    /// Initialize spot.
    /// </summary>
    /// <param name="height"></param>
    /// <param name="_Spots"></param>
    /// <param name="_End"></param>
    public void Initialize(int height, Spot[,] _Spots, Spot _End) {
        this.height = height;
        heuristic = Heuristic(this, _End);

        // Add neigbours
        neighbours = new List<Spot>();
        if (x < _Spots.GetUpperBound(0))
            neighbours.Add(_Spots[x + 1, y]);
        if (x > 0)
            neighbours.Add(_Spots[x - 1, y]);
        if (y < _Spots.GetUpperBound(1))
            neighbours.Add(_Spots[x, y + 1]);
        if (y > 0)
            neighbours.Add(_Spots[x, y - 1]);
    }


    /// <summary>
    /// Get direction from A to B.
    /// </summary>
    public static Vector2 Direction(Spot A, Spot B) => new Vector2(B.x - A.x, B.y - A.y).normalized;


    /// <summary>
    /// Get heuristic of cost to react one spot from another.
    /// </summary>
    public static int Heuristic(Spot A, Spot B) => Math.Abs(A.x - B.x) + Math.Abs(A.y - B.y);


    /// <summary>
    /// Compares two spots by their coordinates.
    /// </summary>
    /// <returns>If same: <see langword="true"/> | else: <see langword="false"/></returns>
    public static bool AreSame(Spot A, Spot B) => A.x == B.x && A.y == B.y;
}