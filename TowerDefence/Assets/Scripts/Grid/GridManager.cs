using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour {

    private Astar astar;
    public static GridManager instance;
    private int[,] tileData;
    private List<Spot> path = new List<Spot>();

    [SerializeField] private GameObject pathColPrefab; // Empty GameObject with layer set to env
    public GameObject pathCollParent { get; private set; }
    public Tilemap groundMap;
    public Tilemap roadMap;
    public Tilemap wallMap;
    public TileBase[] tilePrefabs;
    public GameObject startPrefab;
    public GameObject endPrefab;

    [Header("Map settings"), Range(0.0f, 1.0f)]
    public float scaler = 0.2f;
    [Range(0.0f, 1.0f)]
    public float heightLevel = 0.6f;
    public int width;
    public int height;


    [Header("Path variables")]
    public int roadPoints = 1; // Amount of points between start/end
    public int roadPadding = 3; // Distance from borders where road will be generated

    // This is for tracking placed towers sorting orders, Key is the tower index, Value is the current order
    private Dictionary<int, int> m_CurrentSortingOrders;
    private int startingSortingOrder = 0;
    private int sortingIncrement = 1000;


    private void Awake() {
        if (instance != null) Debug.LogError("[GridManager]: Trying create multiple instaces");
        else instance = this;
    }


    private void Start() {
        GenerateSortingOrders();
        astar = new Astar();
        GenerateMap();
    }


    /// <summary>
    /// Generates starting sorting orders for towers. [Hardcoded tower amount]
    /// </summary>
    private void GenerateSortingOrders() {
        m_CurrentSortingOrders = new Dictionary<int, int>();
        for (int i = 0; i < 6; i++) {
            m_CurrentSortingOrders.Add(i, startingSortingOrder);
            startingSortingOrder += sortingIncrement;
        }
    }

    /// <returns>Mouse position converted to grid position</returns>
    public Vector3Int GetMouseGridPoint() {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return groundMap.WorldToCell(worldPos);
    }


    /// <summary>
    /// Turns grid points to worldpos
    /// </summary>
    /// <param name="p">Vector3Int of grid point.</param>
    /// <returns>vector3 worldposition</returns>
    public Vector3 GridToWorld(Vector3Int p) {
        Vector3 wp = groundMap.CellToWorld(p);
        return wp += groundMap.tileAnchor;
    }


    /// <summary>
    /// Gets mouse position, places tower to that position if position valid
    /// </summary>
    /// <param name="tower">Tower to be placed in grid</param>
    /// <param name="towerIndex"> Towers index for sorting it correctly </param>
    public bool PlaceTower(GameObject tower, int towerIndex) {
        Vector3Int gridPoint = GetMouseGridPoint();
        Vector3 worldPoint = GridToWorld(gridPoint);

        // If not inside map
        if (tileData.GetLength(0) <= gridPoint.x || tileData.GetLength(1) <= gridPoint.y || gridPoint.x < 0 || gridPoint.y < 0) {
            UserNotification.instance.RaiseError(ErrorMsgCode.towerOutsideMapErr);
            return false;
        }

        // If not valid position
        if (tileData[gridPoint.x, gridPoint.y] != Config.TILE_CODE_GROUND) {
            UserNotification.instance.RaiseError(ErrorMsgCode.towerOnPathErr);
            return false;
        }

        // Create and place tower, update tileData
        GameObject spawnedTower = Instantiate(tower, worldPoint, Quaternion.identity);
        // Sort tower
        if (spawnedTower.TryGetComponent<TowerSorter>(out TowerSorter sorter)) {
            if (m_CurrentSortingOrders.TryGetValue(towerIndex, out int currentOrder)) {
                m_CurrentSortingOrders[towerIndex] = sorter.SortTowerOrder(currentOrder);
            }
        } else {
            Debug.LogWarning("[GridManager]: Placed tower did not have sorter component! [" + spawnedTower.name + "]");
        }
        spawnedTower.transform.eulerAngles = new Vector3(0, 0, 0);
        tileData[gridPoint.x, gridPoint.y] = Config.TILE_CODE_TOWER;

        return true;
    }


    /// <summary>
    /// Generates map by looping until valid map given 
    /// </summary>
    private void GenerateMap() {
        // Generate map, try again few times if failed
        int i = 0;
        while (i < 10000) {
            i++;
            // Fill map with default tiles
            GenerateEnv();
            // Generate road with random start/end points, break if successfull
            if (GeneratePath(roadPoints, roadPadding) == 0)
                break;
        }
        if (i >= 10000) Debug.LogError("[GridManager]: Error, 10k iteration limit exceeded");

        SetTiles();
        AI.instance.StorePath(path);
    }


    ///<summary>Generates the path, optional parameter for how many points the path has to go through</summary>
    ///<remarks>
    ///<para><param name="numberOfPoints">Number of points between start/end</param></para>
    ///<para><param name="borderWidth">How far from border we want to generate extra points</param></para>
    ///</remarks>
    ///<returns>0 if path generated</returns>
    private int GeneratePath(int numberofPoints, int borderWidth) {

        // Get points for road to pass
        Vector2Int[] points = GetRoadPoints(numberofPoints, borderWidth);
        path = new List<Spot>();

        // Loop trough every point, generate road between them
        for (int i = 0; i < points.Length - 1; i++) {
            // Find and store path between current and next spot
            List<Spot> path = astar.TryGeneratePath(tileData, points[i], points[i + 1]);

            // Astar failed to return path
            if (path == null)
                return -1;

            if (i > 0) path.RemoveAt(0); // Prevent duplicates
            this.path.AddRange(path);

            // Path to tile data
            for (int j = 0; j < path.Count; j++)
                tileData[path[j].x, path[j].y] = 1;
        }

        if (path.Count < 50 || path.Count > 60) return -1;

        return 0;
    }


    /// <summary> Creates points for road to go trough using basic random </summary>
    /// <remarks>
    /// <param name="numberOfPoints">Number of points for road to go trough excluding start/end</param>
    /// <param name="borderWidth">Min distance from border for placing points</param>
    /// </remarks>
    /// <returns>Array of Vector2Int coordinates where 0 is start and last item is end</returns>
    Vector2Int[] GetRoadPoints(int numberOfPoints, int borderWidth) {

        Vector2Int[] points = new Vector2Int[2 + numberOfPoints];

        // Start and end points
        points[0] = new Vector2Int(0, Random.Range(0, height));
        points[points.Length - 1] = new Vector2Int(width - 1, Random.Range(0, height));

        // Get x number of random points, stored in array/list
        for (int i = 0; i < numberOfPoints; i++)
            points[1 + i] = new Vector2Int(Random.Range(borderWidth, width - borderWidth), Random.Range(borderWidth, height - borderWidth));

        return points;
    }


    /// <summary>
    /// Generates and sets enviroment tiles to grid using perlin noise.
    /// Stores tiles to tiles
    /// </summary>
    private void GenerateEnv() {
        // Initialize and fill tiles with 0's
        tileData = new int[width, height];

        // Loop trough every position in map grid and set tile according to perlin noise
        int noiseOffset = Random.Range(5, 573121);
        for (int i = 0; i < tileData.GetLength(0); i++) {
            for (int j = 0; j < tileData.GetLength(1); j++) {
                float randHeight = Mathf.PerlinNoise(noiseOffset + i * scaler, noiseOffset + j * scaler);
                if (randHeight > heightLevel)
                    tileData[i, j] = Config.TILE_CODE_HILL;
            }
        }
    }

    public void SetTileToGround(int x, int y) =>
        tileData[x, y] = Config.TILE_CODE_GROUND;


    /// <summary>
    /// Tile data to tilemaps
    /// </summary>
    private void SetTiles() {
        Instantiate(startPrefab, new Vector3(path[0].x, path[0].y, 0) + roadMap.tileAnchor, Quaternion.identity);
        Instantiate(endPrefab, new Vector3(path[path.Count - 1].x, path[path.Count - 1].y, 0) + roadMap.tileAnchor, Quaternion.identity);

        for (int i = 0; i < tileData.GetLength(0); i++)
            for (int j = 0; j < tileData.GetLength(1); j++) {
                if (tileData[i, j] == Config.TILE_CODE_HILL)
                    wallMap.SetTile(new Vector3Int(i, j, 0), tilePrefabs[2]);
                else if (tileData[i, j] == Config.TILE_CODE_PATH)
                    roadMap.SetTile(new Vector3Int(i, j, 0), tilePrefabs[1]);
                else
                    groundMap.SetTile(new Vector3Int(i, j, 0), tilePrefabs[0]);
            }
    }


    /// <summary>
    /// Create collider gameobjects using path coordinates for raycasting 
    /// </summary>
    public void CreatePathColliders() {
        pathCollParent = new GameObject("PathColliders");
        TurretConfigurator.instance.pathCollParent = pathCollParent;
        List<BoxCollider2D> pathColliders = new List<BoxCollider2D>();

        // Initialise temps
        Vector2 dir = DirectionBetweenSpots(path[0], path[1]);
        Vector3 origin = new Vector3(path[0].x, path[0].y, 0);
        float length = 0;

        // Loop trough path, create collider object for every straight path senction
        for (int i = 1; i < path.Count - 1; i++) {
            length++;

            // Get direction of current piece of path, continue no direction change
            Vector2 tempDir = DirectionBetweenSpots(path[i], path[i + 1]);
            if (tempDir == dir) continue;

            // Create collider from temps
            float angle = (180 / Mathf.PI) * Mathf.Atan2(dir.y, dir.x) - 90;
            pathColliders.Add(CreatePathCollider(origin, length, angle));

            origin = new Vector3(path[i].x, path[i].y, 0);
            dir = tempDir;
            length = 0;
        }
        // Last path piece
        pathColliders.Add(CreatePathCollider(origin, length + 1.6f, (180 / Mathf.PI) * Mathf.Atan2(dir.y, dir.x) - 90));

        // Convert list to array, send array to TurretConfigurer
        BoxCollider2D[] temp = new BoxCollider2D[pathColliders.Count];
        for (int i = 0; i < pathColliders.Count; i++)
            temp[i] = pathColliders[i];
        TurretConfigurator.instance.allPathColliders = temp;
    }


    /// <returns>Direction vector from a to b Spot</returns>
    private Vector2 DirectionBetweenSpots(Spot a, Spot b) =>
        new Vector2(b.x - a.x, b.y - a.y).normalized;


    private BoxCollider2D CreatePathCollider(Vector2 s, float d, float r) {
        GameObject colliderObj = Instantiate(pathColPrefab, (Vector3)s + groundMap.tileAnchor, Quaternion.identity);
        colliderObj.transform.parent = pathCollParent.transform;
        colliderObj.transform.Rotate(new Vector3(0, 0, r));
        BoxCollider2D pathCollider = colliderObj.AddComponent<BoxCollider2D>();
        pathCollider.size = new Vector2(0.6f, d + 0.6f);
        pathCollider.offset = new Vector2(0, d / 2);
        return pathCollider;
    }
}