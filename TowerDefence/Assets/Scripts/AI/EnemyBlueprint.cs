/// <summary>
/// Blueprint for enemy.
/// </summary>
public struct EnemyBlueprint {

    /// <summary>
    /// Apmount of hitpoints.
    /// </summary>
    public float hp;

    /// <summary>
    /// Movement speed.
    /// </summary>
    public float speed;

    /// <summary>
    /// Reward for killing the enemy.
    /// </summary>
    public int reward;

    /// <summary>
    /// balancing value.
    /// </summary>
    public int weight;

    /// <summary>
    /// Is enemy immune to slow effects by towers.
    /// </summary>
    public bool isImmuneToSlow;

    /// <summary>
    /// Does enemy increase it's movement speed for short duration when it takes damage. 
    /// </summary>
    public bool canSprint;

    /// <summary>
    /// Does enemy heal if not full hp. 
    /// </summary>
    public bool canHeal;

    /// <summary>
    /// Index specifying the type of enemy.
    /// </summary>
    public int index;
}