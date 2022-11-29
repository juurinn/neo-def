using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle beam of turret.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class Beam : MonoBehaviour {

    /// <summary>
    /// ParticleSystems that should be played when beam is enabled.
    /// </summary>
    private List<ParticleSystem> particleSystems;

    /// <summary>
    /// Container of <see cref="particleSystems"/>.
    /// </summary>
    private GameObject particleSystemContainer;

    /// <summary>
    /// The beam.
    /// </summary>
    private LineRenderer _LineRenderer;

    /// <summary>
    /// Owner of the beam. 
    /// </summary>
    private Turret _Turret;

    /// <summary>
    /// Target of the beam. 
    /// </summary>
    private Transform target { get => _Turret.currentTarget; }

    /// <summary>
    /// Origin position of the beam.
    /// </summary>
    private Vector2 origin;

    /// <summary>
    /// Effect of the beam if there is one.
    /// </summary>
    private Coroutine _Coroutine;
    private IEnumerator growing;

    private Coroutine shrinking;

    // Growth timings, in seconds.
    private const float appearTime = 0.125f;
    private const float disappearTime = 0.125f;

    private bool _growing = false;
    private AnimationCurve _laserFadeCurve;
    private const float kMinThickness = 25f;
    private float _eval;
    private Material _Material;
    private float _timer = 0f;
    private float _timerMax = 0f;
    private bool isDisabled;

    private void OnDestroy() {
        Disable();
        StopAllCoroutines();
    }


    private void Awake() {
        origin = transform.position;
        _Turret = GetComponentInParent<Turret>();
        _LineRenderer = GetComponent<LineRenderer>();
        _Material = _LineRenderer.material;

        // Create particlesystems from prefab and get references for all ParticleSystems in it
        particleSystemContainer = transform.GetChild(0).gameObject;
        particleSystems = particleSystemContainer.GetComponentsInChildren<ParticleSystem>().Where(_ParticleSystem => _ParticleSystem != null).ToList();
    }


    private void Update() {
        if (Pause.instance.IsGamePaused) return;
        // If no target or beam not enabled
        if (target == null || (!_Turret.blueprint.laser.beamEnabled && !_Turret.blueprint.slow.beamEnabled)) return;

        Vector2 targetPos = target.position;


        // If target is behind wall, out of range or destroyed
        if (!_Turret.IsAngleWithinLimits || !target.gameObject.activeInHierarchy || (origin - targetPos).magnitude > _Turret.blueprint.range + 0.1f) {
            Disable();
            return;
        }

        UpdateEffect(targetPos);
    }


    /// <summary>
    /// Enable the beam effect for short duration. 
    /// </summary>
    public void EnableForDuration(Vector2 target, float duration) {
        ActivateEffects(target);
        StartCoroutine(DisableAfter(duration));
    }


    /// <summary>
    /// Enable the beam.
    /// </summary>
    public void Enable() {
        ActivateEffects(target.position);
        Enemy _Enemy = target.GetComponent<Enemy>();
        
        // Start beam effect on enemy
        if(_Coroutine != null) StopCoroutine(_Coroutine);
        _Coroutine = StartCoroutine(_Enemy.BeamEffect(_Turret));
    }


    /// <summary>
    /// Disable the beam.
    /// </summary>
    public void Disable() {
        if (isDisabled || !gameObject.activeInHierarchy) return;
        isDisabled = true;


        if (_growing) {
            StopCoroutine(growing);
            _growing = false;
            shrinking = StartCoroutine(ShrinkLaser(disappearTime, true));
        } else {
            shrinking = StartCoroutine(ShrinkLaser(disappearTime));
        }

        // Stop effect on enemy
        if (_Coroutine != null)
            StopCoroutine(_Coroutine);
        // Remove slow if cryo and target exists
        if (_Turret.blueprint.slow.beamEnabled && target != null)
            target.GetComponent<Enemy>().RemoveSlow(_Turret);

        _Turret.SetTargetNull();
        particleSystems.ForEach(_ParticleSystem => _ParticleSystem.Stop());
    }


    /// <summary>
    /// Activate effects of the beam.
    /// </summary>
    private void ActivateEffects(Vector2 target) {
        isDisabled = false;
        if (_growing) Debug.LogWarning("[Beam]: Trying to activate beam while beam is still animating, Check timings!");
        if (shrinking != null) StopCoroutine(shrinking);
        // Enable and set positions of linerenderer
        _LineRenderer.enabled = true;
        _LineRenderer.SetPosition(0, transform.position);
        _LineRenderer.SetPosition(1, target);

        growing = GrowLaser(appearTime);
        StartCoroutine(growing);

        particleSystemContainer.transform.position = target;
        particleSystems.ForEach(_ParticleSystem => _ParticleSystem.Play());
    }


    /// <summary>
    /// Update the beam effect.
    /// </summary>
    private void UpdateEffect(Vector2 _TargetPosition) {
        particleSystemContainer.transform.position = _TargetPosition;
        _LineRenderer.SetPosition(1, _TargetPosition);
    }


    /// <summary>
    /// Disable the beam after duration.
    /// </summary>
    private IEnumerator DisableAfter(float duration) {
        yield return new WaitForSeconds(duration);
        Disable();
    }


    /// <summary>
    /// Grows laser graphic.
    /// </summary>
    /// <param name="duration"> How long does it take to grow the laser to max size, in seconds. </param>
    private IEnumerator GrowLaser(float duration) {
        _growing = true;
        _timer = duration;
        _timerMax = duration;
        _laserFadeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(duration, kMinThickness));

        while (_timer > 0f) {
            _timer -= Time.deltaTime;
            _eval = _laserFadeCurve.Evaluate(_timer);
            _Material.SetFloat("_Thickness", _eval);
            yield return null;
        }

        _timer = 0f;
        _eval = _laserFadeCurve.Evaluate(_timer);
        _Material.SetFloat("_Thickness", _eval);
        _growing = false;
    }


    /// <summary>
    /// Shrinks laser graphic and disables LineRenderer after.
    /// </summary>
    /// <param name="duration"> How long does it take to shrink it, in seconds. </param>
    /// <param name="useCurrentThickness"> If we were growing the size we should take into account the current thickness instead. </param>
    private IEnumerator ShrinkLaser(float duration, bool useCurrentThickness = false) {
        if (!useCurrentThickness) {
            _timer = duration;
            _laserFadeCurve = new AnimationCurve(new Keyframe(0f, kMinThickness), new Keyframe(duration, 0f));
        } else {
            _laserFadeCurve = new AnimationCurve(new Keyframe(0f, kMinThickness), new Keyframe(_timerMax, 0f));
        }

        while (_timer > 0f) {
            _timer -= Time.deltaTime;
            _eval = _laserFadeCurve.Evaluate(_timer);
            _Material.SetFloat("_Thickness", _eval);
            yield return null;
        }

        if (_growing) Debug.LogError("BUG");
        _LineRenderer.enabled = false;
    }
}
