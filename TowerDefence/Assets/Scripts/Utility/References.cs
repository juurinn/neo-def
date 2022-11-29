using UnityEngine;

/// <summary>
/// References to various things.
/// </summary>
public class References : MonoBehaviour {

    static public References instance;

    [Tooltip("This is for parenting spawned particles to something so they don't clutter hierarchy.")]
    public Transform particleParent;

    [Header("Filtering and masking")]

    [Tooltip("For finding out targets.")] public LayerMask targetingLayer;

    [Tooltip("For finding out enemies.")] public LayerMask hitDetectionLayer;

    [Tooltip("Filter out what enemies.")] public ContactFilter2D enemyFilter;

    [Tooltip("Filter out what can be hit by laser, bullets etc")] public ContactFilter2D envFilter;

    [Header("Bleed")]
    public GameObject Shurikens;

    [Header("VisionCone")]
    [SerializeField] private GameObject coneOfVisionPrefab;

    [Header("Dev")]
    public GameObject damageCounterPrefab;

    /// <summary>
    /// Cone used to represent vision of <see cref="Turret"/>
    /// </summary>
    public static ConeOfVision2D coneOfVision;

    /// <summary>
    /// Cone used to represent vision of <see cref="Turret"/> after upgrade.
    /// </summary>
    public static ConeOfVision2D coneOfVisionUpgrade;


    private void Awake() {
        if (instance != null) {
            Debug.LogError("[References]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Create VisionCones
        var coneParent = new GameObject("ConeVisions").transform;
        coneOfVision = Instantiate(coneOfVisionPrefab, coneParent).GetComponent<ConeOfVision2D>();
        coneOfVisionUpgrade = Instantiate(coneOfVisionPrefab, coneParent).GetComponent<ConeOfVision2D>();
    }
}
