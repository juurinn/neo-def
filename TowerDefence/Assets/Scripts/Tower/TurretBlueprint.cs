public enum TowerType {
    Basic, Laser, Cryo, Bleed, Javelin, Pulse
}

/// <summary>
/// Blueprint for turret.
/// Used for base stats and upgrade tiers.
/// </summary>
public struct TurretBlueprint {

    /// <summary>
    /// Name of the turret.
    /// </summary>
    public string name;

    /// <summary>
    /// Cost of the turret/upgrade.
    /// </summary>
    public int cost;

    /// <summary>
    /// Damage of the turret projectile.
    /// </summary>
    public float dmg;

    /// <summary>
    /// How many times turret fires in a second.
    /// </summary>
    public float fireRate;

    /// <summary>
    /// Current range fo the turret.
    /// </summary>
    public float range;

    /// <summary>
    /// Does the turret deal AOE damage.
    /// </summary>
    public bool splashProjectile;

    /// <summary>
    /// 0 = single target, bigger than 0 = splash
    /// </summary>
    public float splashRadius;

    /// <summary>
    /// Description of the tower
    /// </summary>
    public string description;

    /// <summary>
    /// Should tower inflict melee damage.
    /// </summary>
    public bool meleeDmg;

    public LaserProperties laser;
    public DotProperties dot;
    public SlowProperties slow;
    public PiercingProperties piercing;

    /// <summary>
    /// Type of the tower. Means that the type of the tower is type 
    /// </summary>
    public TowerType type;
};


/// <summary>
/// 
/// </summary>
public struct PiercingProperties {
    public bool enabled;
    public float chance;
}


/// <summary>
/// All the neccessary properties to handle laser.
/// </summary>
public struct LaserProperties {
    public bool enabled;
    public bool beamEnabled;
    public int penetration;
    public float beamMultiplier;
}


/// <summary>
/// All the neccessary properties to handle damage over time.
/// </summary>
public struct DotProperties {
    public bool enabled;
    public float damage;
    public int amount;
    public float frequency;
    public float totalDmg { get => amount * damage; }
}


/// <summary>
/// All the neccessary properties to handle slow effect.
/// </summary>
public struct SlowProperties {
    public bool enabled;
    public bool beamEnabled;
    public float time;
    public float multiplier;
}