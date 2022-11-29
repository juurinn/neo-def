using UnityEngine;

/// <summary>
/// Sets tower sorting orders while placing tower down.
/// </summary>
public class TowerSorter : MonoBehaviour {

    // Base should be always 0 since it will be kinda like background element
    [Header("References")]
    [Tooltip("Barrel Sprite Renderer goes here.")]
    [SerializeField] private SpriteRenderer barrel;
    [Tooltip("Energy bar Sprite Renderer goes here.")]
    [SerializeField] private SpriteRenderer energy;

    [Header("Laser Tower")]
    [Tooltip("Laser towers Line Renderer goes here.")]
    [SerializeField] private LineRenderer laser;

    [Header("Javlin Tower")]
    [Tooltip("Javlin towers shooting effect goes here.")]
    [SerializeField] private GameObject javelinEffect;

    [Header("Bleed Tower")]
    [Tooltip("Bleed tower melee object goes here.")]
    [SerializeField] private SpriteRenderer bleedMelee;
    [SerializeField] private SpriteRenderer bleedMeleeBarrel;
    [SerializeField] private SpriteRenderer bleedMeleeEnergy;

    /// <summary>
    /// Uses given sorting order to set sorting orders for this tower.
    /// </summary>
    /// <param name="currentOrder"> Current sorting order for towers. </param>
    /// <returns> Changed current sorting order. </returns>
    public int SortTowerOrder(int currentOrder) {
        int changedOrder = currentOrder;

        if (laser != null) {
            laser.sortingOrder = ++changedOrder;
        }

        if (bleedMelee != null && bleedMeleeBarrel != null && bleedMeleeEnergy != null) {
            bleedMelee.sortingOrder = ++changedOrder;
            bleedMeleeBarrel.sortingOrder = ++changedOrder;
            bleedMeleeEnergy.sortingOrder = ++changedOrder;
        }

        if (javelinEffect != null) {
            javelinEffect.GetComponent<ParticleSystemRenderer>().sortingOrder = ++changedOrder;
        }

        if (barrel != null) {
            barrel.sortingOrder = ++changedOrder;
        }

        if (energy != null) {
            energy.sortingOrder = ++changedOrder;
        }

        return changedOrder;
    }

}
