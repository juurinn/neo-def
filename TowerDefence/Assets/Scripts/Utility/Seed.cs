using UnityEngine;

/// <summary>
/// Manage random seed which determines outcome of map. 
/// </summary>
public class Seed : MonoBehaviour {

    public static Seed instance;

    /// <summary>
    /// Seed for playing specific map.
    /// </summary>
    public int customSeed { private get; set; }

    /// <summary>
    /// Current seed.
    /// </summary>
    public int seed { private set; get; }

    public bool shouldStartGameOnLoad;


    private void Awake() {
        // Keep the first instance
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
    }


    /// <summary>
    /// Use custom seed if one given, else create random seed.
    /// </summary>
    public void CreateSeed() {
        if (customSeed != 0) {
            seed = customSeed;
            customSeed = 0;
        } else
            seed = Random.Range((int)System.UInt32.MinValue, System.Int32.MaxValue);
    }
}
