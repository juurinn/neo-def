using UnityEngine;

/// <summary>
/// Handle hitpoint meshes of the enemy.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HitpointBar : MonoBehaviour {

    [Header("Mesh settings")]
    [Tooltip("Width of the hp bar")] public float meshWidth = 0.6f;
    [Tooltip("Height of the hp bar")] public float meshHeight = 0.15f;
    [Tooltip("Distance from center of the enmy to bottom of the hp bar")] public float yOffset = 0.5f;

    [Header("Refs")]
    [SerializeField] private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;

    private bool shouldCreateTriangles = true; // Create mesh on first hit

    /// <summary>Lowest point of the mesh.</summary>
    private float yBoundMin {
        get { return yOffset; }
    }
    /// <summary>Highest point of the mesh.</summary>
    private float yBoundMax {
        get { return yOffset + meshHeight; }
    }
    /// <summary>Rigth most point of the mesh.</summary>
    private float xBoundMax {
        get { return meshWidth / 2; }
    }
    /// <summary>Left most point of the mesh.</summary>
    private float xBoundMin {
        get { return -xBoundMax; }
    }


    private void Awake() {
        // Init empty mesh
        m_Mesh = new Mesh();
        m_MeshFilter.mesh = m_Mesh;
    }


    private void OnDisable() {
        // Reset mesh before returning enemy to pool
        m_Mesh.Clear();
        shouldCreateTriangles = true;
    }


    /// <summary>
    /// Create triangles for submeshes.
    /// </summary>
    private void CreateTriangles() {
        m_Mesh.subMeshCount = 2;
        m_Mesh.SetTriangles(new int[] { 0, 1, 3, 1, 2, 3 }, 0); // Main mesh
        m_Mesh.SetTriangles(new int[] { 3, 2, 5, 2, 4, 5 }, 1); // Bg mesh

        shouldCreateTriangles = false;
    }


    /// <summary>
    /// Update hp bar mesh.
    /// </summary>
    /// <param name="fillAmount">0 = empty, 1 = full</param>
    public void SetFill(float fillAmount) {
        // Update the mesh if fill amount within bounds
        if (fillAmount < 0 || 1 < fillAmount) { Debug.LogError("[HitpointBar]: Fill amount out of bounds"); return; }

        // Update vertices
        UpdateMesh(fillAmount);

        // Create triangles if not created yet
        if (shouldCreateTriangles) CreateTriangles();
    }


    /// <summary>
    /// Update vertice positions.
    /// </summary>
    /// <param name="fillAmount"></param>
    private void UpdateMesh(float fillAmount) {
        // Calculate fill
        float xFill = xBoundMin + fillAmount * meshWidth;

        m_Mesh.vertices = new Vector3[] {
            new Vector3(xBoundMin, yBoundMin, 0),
            new Vector3(xBoundMin, yBoundMax, 0),
            new Vector3(xFill, yBoundMax, 0),
            new Vector3(xFill, yBoundMin, 0),
            new Vector3(xBoundMax, yBoundMax, 0),
            new Vector3(xBoundMax, yBoundMin, 0)
        };
    }
}
