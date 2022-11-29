using UnityEngine;

/// <summary>
/// Rotates bullets Z axis.
/// </summary>
public class BulletRotation : MonoBehaviour {

    [Tooltip("Is rotation enabled for this bullet.")]
    public bool zEnabled = false;
    [Tooltip("How fast does the bullet rotate.")]
    public float zSpeed = 150;

    private void Update() {
        if (zEnabled) {
            gameObject.transform.Rotate(0, 0, zSpeed * Time.deltaTime);
        }
    }
}
