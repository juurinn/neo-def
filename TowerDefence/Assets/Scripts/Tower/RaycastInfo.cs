using UnityEngine;

/// <summary>
/// Raycast enemy information
/// </summary>
[System.Serializable]
public class RaycastInfo {

    public GameObject m_Enemy;
    public float m_Distance;

    /// <summary>
    /// Information about the enemy hit by a raycast
    /// </summary>
    /// <param name="enemy"> Enemy GameObject </param>
    /// <param name="distance"> Enemy distance to the raycast origin </param>
    public RaycastInfo(GameObject enemy, float distance) {
        m_Enemy = enemy;
        m_Distance = distance;
    }

}
