using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Disables line renderer after set amount of seconds and handles laser VFX.
/// </summary>
public class LaserDisabler : MonoBehaviour {

    public LineRenderer m_LineRenderer;
    //public GameObject laserStartEffect;
    public GameObject laserEndEffect;

    public AnimationCurve laserFadeCurve;

    private float timer = 0f;
    private Material m_Material;
    private List<ParticleSystem> particles = new List<ParticleSystem>();

    private void Start() {
        m_Material = m_LineRenderer.material;
        FillList();
        DisableLaser();
    }

    /// <summary>
    /// Enables laser for set amount of seconds.
    /// </summary>
    /// <param name="disableAfter"> How many seconds the laser is visible. </param>
    /// <param name="useParticles"> Use laser particles. </param>
    /// <param name="endParticlePos"></param>
    public void EnableLaser(float disableAfter, bool useParticles, Vector2 endParticlePos) {
        timer = disableAfter;

        //for (int i = 0; i < laserFadeCurve.length; i++) {
        //    laserFadeCurve.RemoveKey(i);
        //}



        if (useParticles) {
            //laserStartEffect.transform.position = (Vector2)m_LineRenderer.GetPosition(0);
            laserEndEffect.transform.position = endParticlePos;
            for (int i = 0; i < particles.Count; i++) {
                particles[i].Play();
            }
        }
    }

    /// <summary>
    /// Disables laser and VFX.
    /// </summary>
    public void DisableLaser() {
        m_LineRenderer.enabled = false;
        m_Material.SetFloat("_Thickness", 0f);
        for (int i = 0; i < particles.Count; i++) {
            particles[i].Stop();
        }
    }

    private void Update() {
        if (Pause.instance.IsGamePaused) return;
        timer -= Time.deltaTime;

        if (m_LineRenderer.enabled) {
            float eval = laserFadeCurve.Evaluate(timer);
            m_Material.SetFloat("_Thickness", eval);
            if (timer <= 0) {
                DisableLaser();
            }
        }

    }

    /// <summary>
    /// Gets all particle systems for VFX and adds them to a list.
    /// </summary>
    private void FillList() {

        //for (int i = 0; i < laserStartEffect.transform.childCount; i++) {
        //    var particleSystem = laserStartEffect.transform.GetChild(i).GetComponent<ParticleSystem>();
        //    if (particleSystem != null) {
        //        particles.Add(particleSystem);
        //    }
        //} 

        for (int i = 0; i < laserEndEffect.transform.childCount; i++) {
            var particleSystem = laserEndEffect.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (particleSystem != null) {
                particles.Add(particleSystem);
            }
        }
    }

}
