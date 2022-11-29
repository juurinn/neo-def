using UnityEngine;

/// <summary>
/// Gives visual feedback to player when tower is ready to shoot again.
/// </summary>
public class TowerEnergyBar : MonoBehaviour {

    [Tooltip("Energy bar owner tower goes here.")]
    [SerializeField] private Turret m_Turret;

    [Tooltip("Energy bar renderer goes here.")]
    [SerializeField] private SpriteRenderer m_Renderer;

    private void Start() {
        UpdateEnergyValue();
    }

    private void Update() {
        // Update energy meter only when cooldown is active
        if (m_Turret.fireCooldown >= 0f) {
            UpdateEnergyValue();
        }
    }

    /// <summary>
    /// Updates energy bar.
    /// </summary>
    private void UpdateEnergyValue() {
        m_Renderer.material.SetFloat("_EnergyMeter", Mathf.InverseLerp(1, 0, m_Turret.fireCooldown / (1f / m_Turret.blueprint.fireRate)));
    }

}
