using UnityEngine;

/// <summary>
/// Handles swapping tower base sprites when upgrading tower.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class UpgradeSpriteSwap : MonoBehaviour {

    [Tooltip("Upgrade sprites go here.")]
    public Sprite[] upgradeSprites;

    private SpriteRenderer m_SpriteRenderer;

    private void Start() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.sprite = upgradeSprites[0];
    }

    /// <summary>
    /// Swaps base sprite to another one.
    /// </summary>
    /// <param name="index"> The upgrade index </param>
    public void SwapBaseSprite(int index) {
        if (index <= upgradeSprites.Length) {
            m_SpriteRenderer.sprite = upgradeSprites[index];
        } else {
            Debug.LogError("[UpgradeSpriteSwap]: Trying to swap sprite to an index that is not defined!");
        }
    }

}
