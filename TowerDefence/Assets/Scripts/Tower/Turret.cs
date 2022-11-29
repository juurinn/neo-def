using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public enum TargetingMode {
    First, // Most distance traveled
    Last, // Least distance traveled
    Weak, // Enemy that has the least hp
    Strong, // Enemy that has the most hp
    Closest // Closest enemy to the tower
}

/// <summary>
/// Base class for turrets.
/// </summary>
public class Turret : MonoBehaviour {

    private const string CUSTOM_LOOP = "UpdateLoop";

    [Header("Raycast Settings")]
    public float raycastUpdateFrequency = 0.5f; // Time between raycasts, higher = better performance but lower accuracy

    [Header("Turret Rotation")]
    public bool enableRotation = false;
    public Transform partToRotate;
    public float turnSpeed = 10f;

    [Tooltip("Particle effect to play when turret fires.")]
    public ParticleSystem fireParticles;

    [Header("Bullet Settings")]
    public Transform projectileSpawnPoint;
    public GameObject projectileToSpawn;
    public float projectileSpeed;

    [Header("Experiemental")]
    [SerializeField] private GameObject meleeObject;
    [SerializeField] private Shurikens _Shurikens;
    public bool inRangeCheck = false; // Check if enemy is inside the range
    [SerializeField] private bool DrawRay;

    private enum FireType {
        Bullet,
        Laser,
        Beam,
        Melee
    }

    private BulletPool bulletPool;
    private UpgradeSpriteSwap _UpgradeSpriteSwap;
    private Beam _Beam;
    private FireType _FireType;
    private List<(Vector2, Vector2)> raycastPoints;
    private List<(float, float)> rotLimits;

    [HideInInspector] public TargetingMode m_TargetingMode;

    /// <summary>
    /// Index specifying the type of tower. 
    /// </summary>
    [field: SerializeField] public int towerIndex { get; private set; }

    /// <summary>
    /// Current stats and other data of the tower.
    /// </summary>
    public TurretBlueprint blueprint;

    /// <summary>
    /// Index representing current upgrade tier of the tower, 0 if no upgrades. 
    /// </summary>
    public int upgradeTier { get; private set; } = 0;

    /// <summary>
    /// Current target of the enemy, null if no target. 
    /// </summary>
    public Transform currentTarget { get; private set; }

    /// <summary>
    /// Current selling price for this tower.
    /// </summary>
    public int sellPrice { get; private set; } = 0;

    /// <summary>
    /// Current amount of time in seconds before tower is able to shoot again. 
    /// </summary>
    public float fireCooldown { get; private set; } = 0f;

    /// <summary>
    /// Is turret facing the target. Has small tolerance.
    /// </summary>
    private bool IsFacingTarget { get => Vector2.Dot(partToRotate.right, (currentTarget.position - partToRotate.position).normalized) > 0.95f; }

    /// <summary>
    /// Is angle from tower to enemy within vision limits.
    /// </summary>
    public bool IsAngleWithinLimits {
        get => rotLimits.Any(r => isBetween(r.Item1 - 1, r.Item2 + 1, Vector2.SignedAngle(Vector2.right, currentTarget.position - transform.position)));
    }

    bool isBetween(float start, float end, float mid) {
        end = (end - start) < 0.0f ? end - start + 360.0f : end - start;
        mid = (mid - start) < 0.0f ? mid - start + 360.0f : mid - start;
        return (mid < end);
    }

    public float totalDmg;


    private void OnDestroy() {
        CancelInvoke(CUSTOM_LOOP);
    }


    private void Awake() {
        _UpgradeSpriteSwap = GetComponent<UpgradeSpriteSwap>();
        bulletPool = GetComponentInChildren<BulletPool>();

        if (GameManager.instance.devMode) Instantiate(References.instance.damageCounterPrefab, transform);

    }


    private void Start() {
        Configure();

        if (towerIndex == 1) {
            _Beam = GetComponentInChildren<Beam>();
        } else if (towerIndex == 2) {
            _Beam = GetComponentInChildren<Beam>();
        }

        InvokeRepeating(CUSTOM_LOOP, 0f, raycastUpdateFrequency);
    }


    private void Update() {
        if (Pause.instance.IsGamePaused) return;

        // If firing beam, use custom update for it
        if (FireType.Beam == _FireType) { BeamUpdate(); return; }

        fireCooldown -= Time.deltaTime;

        // If no target
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy) return;

        // If enabled, rotate to look at target
        if (enableRotation) {
            RotateTurret();
            if (!IsFacingTarget) 
                return;
        }

        // Fire if cooldown is over
        if (fireCooldown <= 0f) {
            // Don't if in range check is enabled and enemy not in range
            if (inRangeCheck && (transform.position - currentTarget.transform.position).sqrMagnitude > blueprint.range * blueprint.range) { print("Not in range!"); return; }

            Fire();
        }
    }


    /// <summary>
    /// Custom update loop for raycasts and getting target.
    /// </summary>
    private void UpdateLoop() {
        GetTarget();
    }


    /// <summary>
    /// Handle update for <see cref="FireType.Beam"/>.
    /// </summary>
    private void BeamUpdate() {
        if (currentTarget != null) {
            Debug.DrawLine(transform.position, currentTarget.position, Color.cyan);

            if (enableRotation) {
                RotateTurret();
                if (!IsFacingTarget) return;
                Vector3 dir = currentTarget.position - transform.position;
                partToRotate.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);
            }

            if (fireCooldown <= 0) Fire();

            return;
        };

        if (fireCooldown > 0)
            fireCooldown -= Time.deltaTime;
        else
            GetTarget();
    }


    /// <summary>
    /// Handle changing <see cref="FireType"/>.
    /// </summary>
    private void SetFireType(FireType newFireType) {
        switch (newFireType) {
            case FireType.Beam:
                CancelInvoke("UpdateLoop");
                currentTarget = null;
                break;
            case FireType.Melee:
                partToRotate.gameObject.SetActive(false);
                if (meleeObject != null) {
                    meleeObject.SetActive(true);
                } else {
                    Debug.LogWarning("[Turret]: Trying to set fire type to melee but no melee object is set!");
                }
                enableRotation = false;
                break;
        }

        _FireType = newFireType;
    }


    /// <summary>
    /// Fire projectile at the current target.
    /// </summary>
    private void Fire() {
        if (fireParticles != null) fireParticles?.Play();

        switch (_FireType) {
            case FireType.Bullet:
                FireBullet();
                break;
            case FireType.Laser:
                FireLaser();
                break;
            case FireType.Melee:
                _Shurikens.Fire(this, blueprint.range);
                break;
            case FireType.Beam:
                _Beam.Enable();
                break;
            default:
                Debug.LogError("[Turret]: Invalid fire type!");
                break;
        }

        fireCooldown = 1f / blueprint.fireRate;
    }


    /// <summary>
    /// Fires a bullet from the bullet pool.
    /// </summary>
    private void FireBullet() {
        GameObject projectile = bulletPool.GetGameObjectFromPool();
        if (projectile == null) {
            Debug.LogWarning("[Turret]: Could not get bullet from a pool.");
            return;
        }
        projectile.SetActive(true);
    }


    /// <summary>
    /// Fires a laser and damages all the enemies hit.
    /// </summary>
    private void FireLaser() {
        Vector2 dir = currentTarget.position - transform.position;
        RaycastHit2D[] hits = new RaycastHit2D[blueprint.laser.penetration];

        Physics2D.Raycast(transform.position, dir.normalized, References.instance.envFilter, hits, 50);
        foreach (RaycastHit2D hit in hits)
            if (hit.collider.CompareTag("Wall")) {
                _Beam.EnableForDuration(hit.point, 0.2f);
                return;
            } else {
                hit.collider.GetComponent<Enemy>().OnHit(this);
            }
        _Beam.EnableForDuration(hits.Last().collider.transform.position, 0.2f);

    }


    /// <summary>
    /// Rotates the turret towards current target.
    /// </summary>
    private void RotateTurret() {
        Vector3 dir = currentTarget.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Lerp(partToRotate.rotation, angleAxis, Time.deltaTime * turnSpeed);
        partToRotate.rotation = rotation;
    }


    /// <summary>
    /// Get target enemy. Comment block above is longer and original version of this, I agree its bit more readable.
    /// </summary>
    private bool GetTarget() {
        // If targeting mode first or last, we get first enemy found.
        if (m_TargetingMode == TargetingMode.First || m_TargetingMode == TargetingMode.Last)
            currentTarget = GetEnemyInRange();
        // Else get all enemies and figure out which of them fits the description
        else {
            List<Transform> enemies = GetEnemiesInRange();

            currentTarget = enemies.Count == 0
                ? null
                : m_TargetingMode switch {
                    // Enemy with smallest distance to turret
                    TargetingMode.Closest => enemies.Aggregate((default(Transform), float.MaxValue), (seed, value) => (transform.position - value.position).sqrMagnitude < seed.Item2 ? (value, (transform.position - value.position).sqrMagnitude) : seed).Item1,
                    // Enemy with highest hp
                    TargetingMode.Strong => enemies.Select(enemy => enemy.GetComponent<Enemy>()).Aggregate((seed, value) => value.currentHP > seed.currentHP ? value : (value.currentHP == seed.currentHP ? (value.distanceTraveled > seed.distanceTraveled ? value : seed) : seed)).transform,
                    // Enemy with less hp
                    TargetingMode.Weak => enemies.Select(enemy => enemy.GetComponent<Enemy>()).Aggregate((seed, value) => value.currentHP < seed.currentHP ? value : (value.currentHP == seed.currentHP ? (value.distanceTraveled > seed.distanceTraveled ? value : seed) : seed)).transform,
                    _ => null
                };
        }
        return currentTarget != null;
    }


    /// <summary>
    /// Super fast way to get first/last enemy from turrrets range.
    /// </summary>
    /// <returns>First/Last enemy of turrets range, null if one not found</returns>
    private Transform GetEnemyInRange() {
        Vector2 dir;
        RaycastHit2D hit;

        if (m_TargetingMode == TargetingMode.First) {
            for (int i = 0; i < raycastPoints.Count; i++) {
                // Cast ray between points
                dir = raycastPoints[i].Item2 - raycastPoints[i].Item1;
                hit = Physics2D.Raycast(raycastPoints[i].Item1, dir, dir.magnitude, References.instance.targetingLayer);
                if (DrawRay) Debug.DrawRay(raycastPoints[i].Item1, hit ? dir.normalized * hit.distance : dir, Color.white, raycastUpdateFrequency / 10);
                if (hit) return hit.collider.transform.parent; // Return first enemy found
            }
            return null; // No enemy found in range
        }

        // Same as finding first, just directions reversed
        for (int i = raycastPoints.Count - 1; i >= 0; i--) {
            dir = raycastPoints[i].Item1 - raycastPoints[i].Item2;
            hit = Physics2D.Raycast(raycastPoints[i].Item2, dir, dir.magnitude, References.instance.targetingLayer);
            if (DrawRay) Debug.DrawRay(raycastPoints[i].Item2, hit ? dir.normalized * hit.distance : dir, Color.white, raycastUpdateFrequency / 10);
            if (hit) return hit.collider.transform.parent;
        }
        return null;
    }


    /// <summary>
    /// <para>Get all enemies inside the range by casting rays between predefined spots.</para>
    /// </summary>
    /// <returns> List of enemies that got hit. </returns>
    private List<Transform> GetEnemiesInRange() {
        List<Transform> enemies = new List<Transform>();
        List<RaycastHit2D> results = new List<RaycastHit2D>();

        for (int i = 0; i < raycastPoints.Count; i++) {
            Vector2 dir = raycastPoints[i].Item2 - raycastPoints[i].Item1;
            if (DrawRay) Debug.DrawRay(raycastPoints[i].Item1, dir, Color.white, raycastUpdateFrequency / 10);

            // Cast a ray, if enemies are found add them to the enemy list
            if (Physics2D.Raycast(raycastPoints[i].Item1, dir, References.instance.enemyFilter, results, dir.magnitude) > 0)
                foreach (RaycastHit2D result in results) enemies.Add(result.collider.transform);
        }
        return enemies;
    }


    /// <summary>
    /// Handle initial configuring of the turret.
    /// </summary>
    private void Configure() {
        // Get blueprint of turret.
        blueprint = TurretConfig.Get(towerIndex);
        sellPrice += blueprint.cost;
        // Configure raycast directions for enemy detection.
        raycastPoints = TurretConfigurator.instance.GetRaycastPoints(blueprint.range, transform);
        rotLimits = TurretConfigurator.instance.GetRotationRanges(raycastPoints, transform.position);

        /*print(raycastPoints.Aggregate("RaycastPoints ", (s, p) => s + " " + p));
        print(rotLimits.Aggregate("RotLimits ", (s, r) => s + " " + r));
        rotLimits.ForEach(r => {
            Debug.DrawRay(transform.position, (Vector2)(Quaternion.Euler(0, 0, r.Item1) * Vector2.right) * 5, Color.red, 5f);
            Debug.DrawRay(transform.position, (Vector2)(Quaternion.Euler(0, 0, r.Item2) * Vector2.right) * 5, Color.white, 5f);
        });*/

        if (blueprint.laser.enabled) SetFireType(FireType.Laser);
    }


    /// <summary>
    /// Handle kill. 
    /// </summary>
    public void OnKill() {
        if (blueprint.laser.beamEnabled || blueprint.slow.beamEnabled)
            _Beam.Disable();

        currentTarget = null;
    }


    public void SetTargetNull() => currentTarget = null;

    public void DisableBeam() => _Beam.Disable();


    /// <summary>
    /// Upgrade the turret with new values
    /// </summary>
    public void Upgrade(TurretBlueprint upgrade, int newTier) {
        upgradeTier = newTier;

        _UpgradeSpriteSwap.SwapBaseSprite(upgradeTier);

        if (upgrade.name != null) blueprint.name = upgrade.name;

        if (upgrade.cost != 0) sellPrice += upgrade.cost;

        if (upgrade.dmg != 0) blueprint.dmg = upgrade.dmg;
        if (upgrade.range != 0) blueprint.range = upgrade.range;
        if (upgrade.fireRate != 0) blueprint.fireRate = upgrade.fireRate;

        if (upgrade.splashProjectile != false) blueprint.splashProjectile = upgrade.splashProjectile;
        if (upgrade.splashRadius != 0f) blueprint.splashRadius = upgrade.splashRadius;

        if (upgrade.slow.enabled) blueprint.slow = upgrade.slow;
        if (upgrade.dot.enabled) blueprint.dot = upgrade.dot;
        if (upgrade.piercing.enabled) blueprint.piercing = upgrade.piercing;
        if (upgrade.laser.enabled) blueprint.laser = upgrade.laser;

        if (upgrade.laser.beamEnabled) {
            blueprint.laser = upgrade.laser;
            if (_FireType != FireType.Beam) SetFireType(FireType.Beam);
        }

        if (upgrade.slow.beamEnabled) {
            blueprint.slow = upgrade.slow;
            if (_FireType != FireType.Beam) SetFireType(FireType.Beam);
        }

        // Reconfigure racasts
        if (upgrade.range != 0) {
            raycastPoints = TurretConfigurator.instance.GetRaycastPoints(blueprint.range, transform);
            rotLimits = TurretConfigurator.instance.GetRotationRanges(raycastPoints, transform.position);

            /*print(raycastPoints.Aggregate("RaycastPoints ", (s, p) => s + " " + p));
            print(rotLimits.Aggregate("RotLimits ", (s, r) => s + " " + r));
            rotLimits.ForEach(r => {
                Debug.DrawRay(transform.position, (Vector2)(Quaternion.Euler(0, 0, r.Item1) * Vector2.right) * 5, Color.red, 5f);
                Debug.DrawRay(transform.position, (Vector2)(Quaternion.Euler(0, 0, r.Item2) * Vector2.right) * 5, Color.white, 5f);
            });*/
        }

        // Melee
        if (upgrade.meleeDmg) if (_FireType != FireType.Melee) SetFireType(FireType.Melee);
        if (_FireType == FireType.Melee && upgrade.range != 0) _Shurikens.Upgrade(blueprint.range);
    }


    // Draw turret range in editor
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, blueprint.range);
        //Gizmos.DrawLine(transform.position, currentTarget.position); // This errors
    }
}
