using UnityEngine;
using System.Collections;

/// <summary>
/// Handle Shurikens damage and effect.
/// </summary>
public class Shurikens : MonoBehaviour {

    public LayerMask enemyMask;

    /// <summary>
    /// Multiply wanted shurikens radius by this.
    /// </summary>
    private const int speedMultiplier = 8;

    /// <summary>
    /// Small shuriken particles.
    /// </summary>
    private ParticleSystem _ParticleSystem;

    /// <summary>
    /// Center shuriken controller.
    /// </summary>
    private Animator _Animator;


    private void Awake() {
        _Animator = GetComponent<Animator>();
        _ParticleSystem = GetComponent<ParticleSystem>();
    }


    public void Upgrade(float radius) {
        // Assign new curve for particle speed to match radius
        AnimationCurve pekkaCurve = new AnimationCurve();
        pekkaCurve.AddKey(0, radius * speedMultiplier);
        pekkaCurve.AddKey(1, -radius * speedMultiplier);
        var velocityOverLifetime = _ParticleSystem.velocityOverLifetime;
        velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(1f, pekkaCurve);
    }


    public void Fire(Turret _Turret, float radius) {
        PlayEffects();

        Collider2D[] hitColliders = new Collider2D[100];
        int results = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hitColliders, enemyMask);

        for (int i = 0; i < results; i++)
            hitColliders[i].GetComponent<Enemy>().OnHit(_Turret);

        //if (results != 0) StartCoroutine(DoDamageAfter(0.1f)); // might wanna add delay
    }




    /// <summary>
    /// Play effects. 
    /// </summary>
    public void PlayEffects() {
        _ParticleSystem.Play();
        StartCoroutine(SpeedAnimatorFor(_ParticleSystem.main.duration));
    }


    /// <summary>
    /// Speed center shuriken for duration.
    /// </summary>
    private IEnumerator SpeedAnimatorFor(float duration) {
        _Animator.speed = 4;
        yield return new WaitForSeconds(duration);
        _Animator.speed = 1;
    }


    private IEnumerator DoDamageAfter(float duration) {
        yield return new WaitForSeconds(duration);
        DoDamage();
    }


    private void DoDamage() {
    }
}
