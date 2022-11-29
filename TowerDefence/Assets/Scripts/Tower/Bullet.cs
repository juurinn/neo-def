using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class for GameObject bullets.
/// </summary>
public class Bullet : MonoBehaviour {

    [Tooltip("Does this bullet spawn particle system that follows it till the enemy is hit.")]
    [SerializeField] private GameObject flightParticles;

    [Tooltip("Does this bullet spawn particles when it hits the target.")]
    [SerializeField] private GameObject hitParticles;

    [Tooltip("Does this bullet rotate to look at target.")]
    [SerializeField] private bool lookAtTarget = false;

    [Tooltip("How many seconds can this bullet fly until it gets automatically disabled.")]
    [SerializeField] private float bulletLifetime = 10;

    [Tooltip("Filter out which colliders this bullet can hit.")]
    [SerializeField] private LayerMask enemyMask;

    [Tooltip("Does the hit particle effect scale with damage radius.")]
    [SerializeField] private bool isHitParticleAreaEffect = false;

    private readonly int maxColliders = 128; // Max number of collisions for OverlapSphere

    private GameObject currentHitParticles;
    private GameObject currentFlightParticles;

    /// <summary>
    /// Owner of the bullet. 
    /// </summary>
    private Turret _Turret;

    /// <summary>
    /// Pool to which the bullet belongs to. 
    /// </summary>
    private BulletPool _BulletPool;

    /// <summary>
    /// Target of the bullet. 
    /// </summary>
    private Transform currentTarget;

    /// <summary>
    /// Direction for bullet to fly, null if bullet is following <see cref="currentTarget"/>. 
    /// </summary>
    private Vector2? flyDirection;

    /// <summary>
    /// Current direction for bullet to fly, either towards the target or predetermined fly direction.
    /// </summary>
    /// <returns>If bullet is not following target: <see cref="flyDirection"/> | Else direction towards the <see cref="currentTarget"/>.</returns>
    private Vector2 Direction { get => flyDirection != null ? flyDirection.Value : (Vector2)(currentTarget.position - transform.position); }

    /// <summary>
    /// Enemies that the bullet has hit. For preventing multiple hits when piercing enabled.
    /// </summary>
    private List<Transform> previousHits = new List<Transform>();

    /// <summary>
    /// Time since bullet was released from pool, excluding paused time.
    /// </summary>
    private float timer = 0f;


    private void OnDisable() {
        timer = 0f;
        flyDirection = null;
        previousHits = new List<Transform>();
    }


    private void OnEnable() {
        if (_Turret == null) return;

        // Set bullet parameters after release from pool
        transform.position = _Turret.projectileSpawnPoint.position;
        transform.rotation = _Turret.projectileSpawnPoint.rotation;
        currentTarget = _Turret.currentTarget;

        // Create new flight particles
        if (flightParticles != null)
            currentFlightParticles = Instantiate(flightParticles, transform.position, Quaternion.identity, References.instance.particleParent);
    }


    private void Update() {
        if (Pause.instance.IsGamePaused) return;
        timer += Time.deltaTime;

        // Destroy if over the lifetime
        if (timer >= bulletLifetime) {
            SpawnHitParticles();
            _BulletPool.ReturnGameObjectToPool(gameObject);
            return;
        }

        // Movement in this frame
        float distance = _Turret.projectileSpeed * Time.deltaTime;
        Vector2 direction = Direction;

        // Handle target hit
        if (flyDirection == null && direction.magnitude <= distance) {
            HandleHit(currentTarget);
            return;
        }
        direction = direction.normalized * distance;

        // Handle piercing
        if (_Turret.blueprint.piercing.enabled) {
            if (HitDetection(direction)) return;
        }

        // Move and rotate
        transform.Translate(direction, Space.World);
        if (lookAtTarget)
            transform.rotation = LookInDirection(currentTarget.position - transform.position);

        // Update flight particle position
        if (flightParticles != null && currentFlightParticles != null)
            currentFlightParticles.transform.position = transform.position;
    }


    /// <summary>
    /// Detect enemy hits with raycast and handle them.
    /// </summary>
    /// <returns>If bullet destroyed due to hit <see langword="true"/> : Else <see langword="false"/></returns>
    private bool HitDetection(Vector2 movement) {
        RaycastHit2D[] hits = new RaycastHit2D[10];

        if (Physics2D.Raycast(transform.position, movement, References.instance.envFilter, hits, movement.magnitude) > 0)
            foreach (Transform hit in hits.Select(h => h.transform)) {
                if (hit == null || previousHits.Contains(hit)) continue;
                if (HandleHit(hit)) return true;
            }

        return false;
    }


    /// <summary>
    /// Handle enemy hit.
    /// </summary>
    //// <returns>If bullet destroyed <see langword="true"/> : Else <see langword="false"/></returns>
    private bool HandleHit(Transform enemy) {
        SpawnHitParticles();

        // If hit wall
        if (enemy.tag == "Wall") {
            _BulletPool.ReturnGameObjectToPool(gameObject);
            return true;
        }

        // Apply damage to the target/targets
        if (_Turret.blueprint.splashRadius > 0f)
            DoSplashDamage();
        else
            DoDamage(enemy);

        // Handle piercing if enabled
        if (_Turret.blueprint.piercing.enabled)
            return HandlePiecing(enemy);

        // Else return bullet to pool
        _BulletPool.ReturnGameObjectToPool(gameObject);
        return true;
    }


    /// <summary>
    /// Handle piercing.
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns>If bullet destroyed <see langword="true"/> : Else <see langword="false"/></returns>
    private bool HandlePiecing(Transform enemy) {
        if (flyDirection == null)
            flyDirection = (enemy.position - _Turret.transform.position).normalized;

        if (Random.value <= _Turret.blueprint.piercing.chance) {
            previousHits.Add(enemy);
            return false;
        }

        _BulletPool.ReturnGameObjectToPool(gameObject);
        return true;
    }


    /// <summary>
    /// Do damage to single enemy.
    /// </summary>
    private void DoDamage(Transform target) {
        if (!target.gameObject.activeInHierarchy) return;

        if (target.TryGetComponent<Enemy>(out Enemy enemy))
            enemy.OnHit(_Turret);
        else
            Debug.LogWarning("[Bullet]: Could not get enemy component while hitting an enemy: [" + currentTarget.gameObject.name + "]!");
    }


    /// <summary>
    /// Get enemies inside splash radius and do damage to them.
    /// </summary>
    private void DoSplashDamage() {
        Collider2D[] hitColliders = new Collider2D[maxColliders];
        int numColliders = Physics2D.OverlapCircleNonAlloc(transform.position, _Turret.blueprint.splashRadius, hitColliders, References.instance.hitDetectionLayer);

        for (int i = 0; i < numColliders; i++)
            DoDamage(hitColliders[i].transform);
    }


    /// <summary>
    /// Tries to spawn hit particles at the current position.
    /// </summary>
    private void SpawnHitParticles() {
        if (hitParticles == null) return;

        currentHitParticles = Instantiate(hitParticles, transform.position, transform.rotation);
        currentHitParticles.transform.parent = References.instance.particleParent;

        // Scale the effect to the damage radius
        if (isHitParticleAreaEffect && _Turret.blueprint.splashRadius > 0f) {
            ParticleSystem.MainModule m_MainModule = currentHitParticles.GetComponent<ParticleSystem>().main;
            m_MainModule.startSize = _Turret.blueprint.splashRadius * 2;
        }
    }


    /// <summary>
    /// This is transform.LookAt equivalent for 2D.
    /// </summary>
    /// <param name="target"> Transform to look at. </param>
    /// <returns> Quaternion the Transform should look at to face the target. </returns>
    private Quaternion LookInDirection(Vector2 direction) {
        Quaternion rotation = Quaternion.LookRotation(direction, transform.TransformDirection(Vector3.up));
        return new Quaternion(0, 0, rotation.z, rotation.w);
    }


    /// <summary>
    /// Set reference to which turret this bullet belongs to.
    /// </summary>
    /// <param name="turret"> The Turret which this bullet belongs to. </param>
    public void SetTurretRef(Turret turret) => _Turret = turret;


    /// <summary>
    /// Set reference to which pool this bullet belongs to.
    /// </summary>
    /// <param name="pool"> The BulletPool which this bullet belongs to. </param>
    public void SetPoolRef(BulletPool pool) => _BulletPool = pool;


    // Draw splash radius in editor
    private void OnDrawGizmosSelected() {
        if (_Turret.blueprint.splashRadius > 0f) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _Turret.blueprint.splashRadius);
        }
    }
}
