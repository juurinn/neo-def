using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle configuring raycasts for the turrets
/// </summary>
public class TurretConfigurator : MonoBehaviour {

    public static TurretConfigurator instance;
    public GameObject pathCollParent { private get; set; }
    public BoxCollider2D[] allPathColliders { private get; set; }

    // Temp variables are declared in global scope and reused for saving garbage
    private List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
    private List<(Vector2, Vector2)> raycastPoints;
    private Transform m_Transform;
    private BoxCollider2D m_BoxCollider2D;
    private Vector2 tempStart;
    private Vector2 tempEnd;
    private Vector2 targetPosition;

    // Turret info 
    private float vRange; // Range of the turrets vision in units
    private float turretRotation; // Euler.z rotation of the turret
    private Vector2 turretPosition; // World position of the turret

    [Header("Settings")]
    [SerializeField, Tooltip("Everything that blocks bullets + pathcolliders")] private ContactFilter2D envFilter;
    private float raycastAmount = 1000;
    private float scanAccuracy = 0.1f;
    private float tolerance = 0.07f;

    [Header("Debug")]
    [SerializeField, Tooltip("Should we draw rays during config")] private bool drawRays;
    [SerializeField, Tooltip("Should we print logs about config")] private bool printLogs;
    [SerializeField, Range(0, 10), Tooltip("Result rays are drawn for duration")] private float resultRayDuration;


    private void Awake() {
        if (instance == null) instance = this;
        else {
            Debug.LogError("[TurretConfigurator]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    public List<(float, float)> GetRotationRanges(List<(Vector2, Vector2)> points, Vector2 pos) {
        if (points.Count() == 0) return new List<(float, float)>();

        var temp = points.Select(p => Range((Angle(p.Item1 - pos), Angle(p.Item2 - pos)))).OrderBy(p => p.Item1).ToList();
        var results = new List<(float, float)>() { temp[0] };

        temp.ForEach(p => {
            var last = results.Last();
            if (last.Item2 + 1 >= p.Item1) results[results.IndexOf(last)] = (last.Item1, Mathf.Max(p.Item2, last.Item2));
            else results.Add(p);
        });

        return results;

        float Angle(Vector2 vekkori) {
            float a = Vector2.SignedAngle(Vector2.right, vekkori);
            return a < 0 ? a + 360 : a;
        }

        (float, float) Range((float, float) range) {
            float min = Mathf.Min(range.Item1, range.Item2);
            float max = Mathf.Max(range.Item1, range.Item2);
            return max - min > 180 ? (max, min + 360) : (min, max);
        }
    }


    /// <summary>
    /// <para>Get positions alongside the path for casting rays between for turret enemy detection.</para>
    /// <para>Find pathColliders the turret is able to hit by casting rays away from the turret between min/max angles of the turret.</para>
    /// <para>Scan every every path collider to determine sections of collider that the turret is able to hit.</para>
    /// </summary>
    /// <param name="m_Turret">Blueprint of turret with all relevant turret information</param>
    /// <param name="_Transform">Transform of the turret</param>
    /// <returns>List of Tuples where item1's are start points and item2's are end points for raycasts</returns>
    public List<(Vector2, Vector2)> GetRaycastPoints(float _Range, Transform _Transform) {
        raycastPoints = new List<(Vector2, Vector2)>(); // Reset

        vRange = _Range;
        turretPosition = _Transform.position;

        // Convert rotation from counter clockwise to clockwise
        turretRotation = _Transform.eulerAngles.z == 0 ? 0 : 360 - _Transform.eulerAngles.z;

        // Get path colliders the turret is able to hit
        // Scan every collider and form "raycast points " from that data
        foreach (Collider2D collider in GetPathColliders()) {
            m_Transform = collider.transform;
            m_BoxCollider2D = collider.GetComponent<BoxCollider2D>();

            // Path is horizontal or not, Im very proud of this temp solution
            bool isHorizontal = (m_Transform.eulerAngles.z < 280 && m_Transform.eulerAngles.z > 260) || (m_Transform.eulerAngles.z < 100 && m_Transform.eulerAngles.z > 80);

            // Get Start/End points of collider for the scan. Start is the point of collider further along the path.
            // If points are outside of the visionZone, limit them
            //Vector3 startPoint = (Vector3)PointToVisionZone(m_Transform.position + m_Transform.up * m_BoxCollider2D.size.y, isHorizontal);
            Vector3 startPoint = (Vector3)PointToVisionZone(m_Transform.position + m_Transform.up * (m_BoxCollider2D.size.y - 0.6f), isHorizontal);
            Vector3 endPoint = (Vector3)PointToVisionZone(m_Transform.position, isHorizontal);

            // Scan collider by casting rays from start to end and form raycastPoints from that data
            ScanCollider(m_BoxCollider2D, startPoint, endPoint);
        }

        if (resultRayDuration > 0) foreach ((Vector2, Vector2) r in raycastPoints) Debug.DrawRay(r.Item1, r.Item2 - r.Item1, Color.white, resultRayDuration);
        return raycastPoints;
    }



    /// <summary>
    /// <para>Cast rays away from the position of the turret in angles between min/max vision angles of the turret.</para>
    /// <para>Raycasts are limited to env layer. Todo: add optional envFilter for wallhack turret </para>
    /// <para>Store every pathcollider found and sort them to ascending order by their position in the path.</para>
    /// </summary>
    /// <returns>List of pathcolliders found that turret is able to hit with raycast</returns>
    private List<Collider2D> GetPathColliders() {
        List<Collider2D> pathColliders = new List<Collider2D>();

        // Loop full circle
        for (float angle = 0; angle < 360 + 0; angle += 360 / raycastAmount) {
            raycastHits.Clear();

            // Cast ray by turning angle to direction vector
            if (Physics2D.Raycast(turretPosition, VecFromAngle(angle), envFilter, raycastHits, vRange) > 0) {
                // Loop trough hits and add all pathcolliders found to list of pathcolliders
                foreach (RaycastHit2D hit in raycastHits) {
                    Collider2D col = hit.collider;
                    if (col.transform.parent.gameObject != pathCollParent) break;
                    if (!pathColliders.Contains(col)) pathColliders.Add(col);
                }
            }
            if (drawRays) Debug.DrawRay(turretPosition, VecFromAngle(angle) * vRange, Color.green, resultRayDuration);
        }

        // Sort pathColliders to ascending order before returning them
        pathColliders.Sort((a, b) => Array.FindIndex(allPathColliders, 0, x => x == b).CompareTo(Array.FindIndex(allPathColliders, 0, x => x == a)));
        return pathColliders;
    }


    /// <summary>
    /// <para>Check if world point is in the vision zone of the turret, limit it if not</para>
    /// </summary>
    /// <param name="worldPoint">World position to be constrained in turrets vision zone</param>
    /// <param name="isHorizontal">Is the target path piece horizontal.</param>
    /// <returns>World point inside of the turrets vision zone.</returns>
    private Vector2 PointToVisionZone(Vector2 worldPoint, bool isHorizontal) {
        // Position of the world point from the turrets perspective
        Vector2 m_Point = worldPoint - turretPosition;
        // Shortest distance from turret to path
        float sDist = Math.Abs(isHorizontal ? m_Point.y : m_Point.x);
        Logger("PointToVisionZone, start: " + worldPoint + " sdist: " + sDist + " startAngle: ");

        // If point inside the range, return point as world position
        if (m_Point.magnitude < vRange + tolerance) return turretPosition + m_Point;

        // Limit direction to inside the range using Pythagorean theorem
        // Range = hypotenusa, Adjacent side = shortest distance to path
        // Opposite side can be derieved with: b^2 = c^2 - a^2
        if (vRange - sDist < tolerance) sDist -= tolerance; // Add tolerance if turret is barely reaching the path
        float b = Mathf.Sqrt((vRange * vRange) - (sDist * sDist));

        // Return new point as world position
        return turretPosition + (isHorizontal
            ? new Vector2(Mathf.Sign(m_Point.x) * b, m_Point.y)
            : new Vector2(m_Point.x, Math.Sign(m_Point.y) * b));
    }


    /// <summary>
    /// <para>Scan collider by casting rays against it from and end to another.</para>
    /// <para>From hit position data, create start/end pairs of path sections that the turret is able to shoot</para>
    /// <para>Results are stored in class field which includes also data from other scans</para>
    /// </summary>
    /// <param name="m_Collider">Collider to be scanned</param>
    /// <param name="startPosition">World position for starting to cast rays</param>
    /// <param name="endPosition">World position for to move towards while casting rays</param>
    private void ScanCollider(BoxCollider2D m_Collider, Vector2 startPosition, Vector2 endPosition) {
        // Edge case. If start ≈ end, create mini ray and return
        if ((startPosition - endPosition).magnitude < 0.1f) { raycastPoints.Add((startPosition, endPosition + Vector2.up * 0.001f)); return; }
        Logger("target: " + startPosition + " goal " + endPosition);
        int i = 0; // Failsafe

        tempStart = Vector2.zero;
        tempEnd = Vector2.zero;
        // Cast rays towards the target position and move towards the end position
        targetPosition = startPosition;
        while (true) {
            if (i > 1000) { Logger("[TurretConfigurator]: Infinite loop on isle 3, call tech support", true); break; } else i++;
            raycastHits.Clear();

            if (Physics2D.Raycast(turretPosition, targetPosition - turretPosition, envFilter, raycastHits, vRange) > 0) {
                foreach (RaycastHit2D hit in raycastHits) {
                    // If not path collider
                    if (hit.transform.parent.gameObject != pathCollParent) {
                        if (tempStart == Vector2.zero) break;
                        raycastPoints.Add((tempStart, tempEnd));
                        tempStart = Vector2.zero;
                        break;
                    }
                    // If the target path collider
                    else if (hit.collider == m_Collider) {
                        if (tempStart == Vector2.zero) tempStart = targetPosition;
                        tempEnd = targetPosition;
                        break;
                    }
                }
            }
            // If not hits, store raycastPoints if ones were in the makings
            else if (tempStart != Vector2.zero) {
                raycastPoints.Add((tempStart, tempEnd));
                tempStart = Vector2.zero;
            }
            // Move target position towards the end position
            if (targetPosition == endPosition) break;
            targetPosition = Vector2.MoveTowards(targetPosition, endPosition, scanAccuracy);
        }
        if (tempStart != Vector2.zero) raycastPoints.Add((tempStart, tempEnd)); // Add the last one
    }


    /// <summary>
    /// <para>Converts angle to direction vector.</para>
    /// <para>Follows unusual convetion starting directions from [0,0] and going in the direction towards [0, -1]</para>
    /// </summary>
    /// <param name="a">Angle to be converted</param>
    /// <returns>Direction vector</returns>
    private Vector2 VecFromAngle(float a) =>
        new Vector2(Mathf.Cos(-a * Mathf.Deg2Rad), Mathf.Sin(-a * Mathf.Deg2Rad));


    /// <summary>
    /// Logs message if logging enabled or error occurred.
    /// </summary>
    /// <param name="msg">Message to be logged</param>
    /// <param name="error">Optional, if true logs error message</param>
    private void Logger(string msg, bool error = false) {
        if (!printLogs && !error) return;
        msg = "[TurretConfigurator]:" + msg;
        if (error) Debug.LogError(msg);
        else Debug.Log(msg);
    }
}
