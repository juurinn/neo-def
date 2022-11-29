using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for enemies.
/// </summary>
public class Enemy : MonoBehaviour {

    /// <summary>
    /// Death particle prefab.
    /// </summary>
    [SerializeField] private GameObject deathParticles;

    /// <summary>
    /// Slow effects that are currently affecting the enemy.
    /// </summary>
    private List<SlowEffectData> slows;
    private struct SlowEffectData {
        public Turret owner; // Owner of the effect
        public Coroutine coroutine; // Store coroutine responsible for cancelling the effect, if there is one
        public float multiplier; // Multiplier for enemy speed

        public SlowEffectData(Turret owner, float multiplier, Coroutine coroutine = null) {
            this.owner = owner;
            this.multiplier = multiplier;
            this.coroutine = coroutine;
        }
    }

    /// <summary>
    /// Current dot affecting the enemy if there is one. 
    /// </summary>
    private Dot dot;
    private struct Dot {
        public Turret owner;
        public Coroutine coroutine;
        public DotProperties props;
        public Dot(Coroutine coroutine, Turret turret, DotProperties properties) {
            this.coroutine = coroutine;
            this.owner = turret;
            this.props = properties;
        }
    }

    /// <summary>
    /// HitPointBar controller of the enemy.
    /// </summary>
    private HitpointBar hitpointBar;

    /// <summary>
    /// Stats of the enemy. 
    /// </summary>
    private EnemyBlueprint _Enemy;

    /// <summary>
    /// Index of the wave enemy belongs.
    /// </summary>
    private int waveIndex;

    /// <summary>
    /// Index specifying the enemy type, used for returning enemy to correct pool, etc...
    /// </summary>
    public int enemyIndex { private get; set; }

    /// <summary>
    /// Last index of the <see cref="path"/> that enemy has passed.
    /// </summary>
    private int currentPathIndex;

    /// <summary>
    /// List of positions forming the path for enemy to follow.
    /// </summary>
    public List<Vector2> path { private get; set; }

    /// <summary>
    /// Current target position of <see cref="path"/>
    /// </summary>
    private Vector2 targetPosition { get => new Vector2(path[currentPathIndex].x, path[currentPathIndex].y); }

    /// <summary>
    /// Has enemy been destroyed due to finishing or dying. 
    /// </summary>
    private bool isDestroyed;


    /// <summary>
    /// Current sprint effect. 
    /// </summary>
    private Coroutine sprintCoroutine;

    /// <summary>
    /// Current multiplier for slowing the enemy, 1 if no effects affecting the enemy.
    /// </summary>
    private float currentSlowMultiplier { get => slows.Count == 0 ? 1 : slows.Min(slow => slow.multiplier); }

    /// <summary>
    /// Current speed multiplier for speed, 1 when not sprinting.
    /// </summary>
    private float currentSprintMultiplier;

    /// <summary>
    /// Current movement speed, enemy speed with effects taken to account.
    /// </summary>
    private float currentSpeed { get => _Enemy.speed * currentSprintMultiplier * currentSlowMultiplier; }

    /// <summary>
    /// Amount of hp enemy currently has. 
    /// </summary>
    public float currentHP { get; private set; }

    /// <summary>
    /// Distance traveled by enemy.
    /// </summary>
    public float distanceTraveled {
        get {
            if (DistanceTraveled != 0) return DistanceTraveled;

            DistanceTraveled = currentPathIndex + Vector2.Distance(new Vector2(path[currentPathIndex].x, path[currentPathIndex].y), transform.position);
            return DistanceTraveled;
        }
    }
    private float DistanceTraveled;


    private void Awake() {
        hitpointBar = GetComponentInChildren<HitpointBar>();
    }


    private void Start() {
        if (_Enemy.canHeal) InvokeRepeating("Heal", 2f, 0.5f);
    }


    private void OnDisable() {
        StopAllCoroutines();
        CancelInvoke();
    }


    private void FixedUpdate() {
        if (Pause.instance.IsGamePaused) return;
        DistanceTraveled = 0;

        // Move towards the target
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentSpeed * Time.fixedDeltaTime);

        // Check if we reached the target
        if ((Vector2)transform.position == targetPosition) {
            currentPathIndex++;
            // If we reached end, return enemy to pool and remove one life from player
            if (currentPathIndex == path.Count)
                HandleFinish();
        }
    }


    /// <summary>
    /// Invoked with custom loop for enemies that are able to heal. 
    /// </summary>
    private void Heal() {
        if (isDestroyed || currentHP == _Enemy.hp) return;

        // Heal by percentage, at most to max hp
        float increasedHP = currentHP + _Enemy.hp * Enemies.HEAL_PERCENTAGE;
        currentHP = increasedHP > _Enemy.hp ? _Enemy.hp : increasedHP;

        hitpointBar.SetFill(currentHP / _Enemy.hp);
    }


    /// <summary>
    /// Handle enemy that reached the target.
    /// </summary>
    private void HandleFinish() {
        // Cannot finish more than once, cannot finish if destroyed during this frame
        if (isDestroyed) return;
        isDestroyed = true;

        GameManager.instance.OnEnemyGotTrough(waveIndex);

        // Handle returning to pool
        StopAllCoroutines();
        EnemyPools.instance.ReturnGameObjectToPool(gameObject, enemyIndex);
    }


    /// <summary>
    /// Add slow effect to enemy, start expiration wait if duration set.
    /// </summary>
    public void AddSlow(Turret owner, float multiplier, float? duration = null) {
        // If previous slows found by same turret, remove them
        slows.Where(slow => slow.owner == owner).ToList().ForEach(slow => {
            if (slow.coroutine != null) StopCoroutine(slow.coroutine);
            slows.Remove(slow);
        });

        // Add slow without coroutine ref
        if (duration == null)
            slows.Add(new SlowEffectData(owner, multiplier));
        // Add slow with coroutine ref
        else
            slows.Add(new SlowEffectData(owner, multiplier, StartCoroutine(RemoveSlowAfterDuration(duration.Value, owner))));
    }


    /// <summary>
    /// Remove slow effect of turret from enemy.
    /// </summary>
    public void RemoveSlow(Turret slowOwner) => slows.Remove(slows.Find(slow => slow.owner == slowOwner));


    /// <summary>
    /// Handle hit.
    /// </summary>
    /// <param name="hitOwner">Owner of the hit.</param>
    public void OnHit(Turret hitOwner) {
        if (!gameObject.activeInHierarchy) return;
        TurretBlueprint turret = hitOwner.blueprint;

        // Apply damage
        if (turret.dmg != 0f && ApplyDamage(turret.dmg, hitOwner))
            return;

        // If enemy should sprint
        if (_Enemy.canSprint) {
            // If already sped up
            if (sprintCoroutine != null)
                StopCoroutine(sprintCoroutine);

            sprintCoroutine = StartCoroutine(IncreaseSpeedForDuration(Enemies.SPRINT_DURATION, Enemies.SPRINT_MULTIPLIER));
        }


        // If should apply Damage over time.
        if (turret.dot.enabled) {
            if (dot.coroutine != null) StopCoroutine(dot.coroutine);
            dot.coroutine = StartCoroutine(DoDamageOverTime(turret.dot, hitOwner));
        }

        // If should, apply slow
        if (turret.slow.enabled && !_Enemy.isImmuneToSlow)
            AddSlow(hitOwner, turret.slow.multiplier, turret.slow.time);
    }


    /// <summary>
    /// Reset values to defaults on enable.
    /// </summary>
    public void Enable(int _waveIndex) {
        waveIndex = _waveIndex;

        // Reset enemy stats to default after spawning from pool
        _Enemy = Enemies.all[enemyIndex];
        _Enemy.hp = WaveConfig.HpMultiplier(enemyIndex, waveIndex);
        currentHP = _Enemy.hp;

        transform.position = new Vector3(path[0].x, path[0].y);
        currentPathIndex = 0;
        isDestroyed = false;

        slows = new List<SlowEffectData>();
        dot = new Dot();
        currentSprintMultiplier = 1;
    }


    /// <summary>
    /// Apply damage to the enemy and kill enemy if needed.
    /// </summary>
    /// <returns>killed the enemy: <see langword="true"/> | else: <see langword="false"/></returns>
    private bool ApplyDamage(float damage, Turret bulletOwner) {
        // Cap damage to current hp
        damage = Mathf.Min(currentHP, damage);

        // Take damage and add to bullet owners total damage
        currentHP -= damage;

        // Check if enemy is dead
        if (currentHP == 0) {
            if (bulletOwner != null) bulletOwner.OnKill(); // Might be null if dot kills enemy after turret has been sold
            KillEnemy();
            return true;
        }

        // Update hp when enemy is still alive
        hitpointBar.SetFill(currentHP / _Enemy.hp);
        return false;
    }


    /// <summary>
    /// Handle enemy death.
    /// </summary>
    private void KillEnemy() {
        if (isDestroyed) return;
        isDestroyed = true;

        StopAllCoroutines();

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, transform.rotation, References.instance.particleParent);

        GameManager.instance.OnEnemyKill(enemyIndex, waveIndex);
        EnemyPools.instance.ReturnGameObjectToPool(gameObject, enemyIndex);
    }


    public IEnumerator BeamEffect(Turret beamOwner) {
        // Laser
        if (beamOwner.blueprint.type == TowerType.Laser)
            return LaserBeamDmg(beamOwner.blueprint.dmg, beamOwner.blueprint.laser.beamMultiplier, beamOwner);
        // Cryo
        else {
            AddSlow(beamOwner, beamOwner.blueprint.slow.multiplier);
            return FreezeBeamDmg(beamOwner.blueprint.dmg, beamOwner);
        }
    }


    /// <summary>
    /// Damage over time from beam, random fomula used atm.
    /// </summary>
    private IEnumerator LaserBeamDmg(float dmg, float tickRate, Turret beamOwner) {
        while (true) {
            dmg += tickRate * dmg;
            tickRate *= 0.95f;

            if (ApplyDamage(dmg * tickRate, beamOwner)) 
                break;
            else
                yield return new WaitForSeconds(tickRate);
        }
    }


    private IEnumerator FreezeBeamDmg(float dmg, Turret beamOwner) {
        const float tickRate = 0.5f;
        while (true)
            if (ApplyDamage(dmg * tickRate, beamOwner))
                break;
            else
                yield return new WaitForSeconds(tickRate);
    }


    /// <summary>
    /// Handle Damage over time to enemy.
    /// </summary>
    private IEnumerator DoDamageOverTime(DotProperties newDot, Turret newOwner) {
        if (newDot.totalDmg > dot.props.totalDmg) {
            dot.props = newDot;
            dot.owner = newOwner;
        }

        while (dot.props.amount > 0) {
            dot.props.amount--;

            if (ApplyDamage(dot.props.damage, dot.owner)) yield break;
            yield return new WaitForSeconds(dot.props.frequency);
        }
    }


    /// <summary>
    /// Increase movement speed for duration.
    /// </summary>
    private IEnumerator IncreaseSpeedForDuration(float duration, float multiplier) {
        currentSprintMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        currentSprintMultiplier = 1;
    }


    /// <summary>
    /// Remove slow effect from the enemy after duration.
    /// </summary>
    private IEnumerator RemoveSlowAfterDuration(float duration, Turret owner) {
        yield return new WaitForSeconds(duration);
        RemoveSlow(owner);
    }
}
