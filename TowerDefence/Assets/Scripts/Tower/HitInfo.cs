/// <summary>
/// Bullet hit information to send to the enemy.
/// </summary>
[System.Serializable]
public class HitInfo {

    public float m_Damage;
    public Turret m_Turret;
    public bool m_ApplyDmgAsDOT;
    public int m_DotTickAmount;
    public float m_DotTickFrequency;
    public bool m_ApplySlow;
    public float m_SlowMultiplier;
    public float m_SlowTime;

    /// <summary>
    /// Information about the bullet hit.
    /// </summary>
    /// <param name="damage"> Damage to do to the target enemy. </param>
    /// <param name="bulletOwner"> Turret that owns this bullet. </param>
    /// <param name="applyDmgAsDOT"> Does the bullet apply the damage amount as damage over time instead. </param>
    /// <param name="dotTickAmount"> How many damage ticks does this bullet apply. </param>
    /// <param name="dotTickFrequency"> How often does the dot tick. </param>
    /// <param name="applySlow"> Does the bullet apply slow. </param>
    /// <param name="slowMultiplier"> How much does this bullet slow the enemy. </param>
    public HitInfo(float damage, Turret bulletOwner, bool applyDmgAsDOT = false, int dotTickAmount = 0, float dotTickFrequency = 0, bool applySlow = false, float slowMultiplier = 0f, float slowTime = 0f) {
        m_Damage = damage;
        m_Turret = bulletOwner;
        m_ApplyDmgAsDOT = applyDmgAsDOT;
        m_DotTickAmount = dotTickAmount;
        m_DotTickFrequency = dotTickFrequency;
        m_ApplySlow = applySlow;
        m_SlowMultiplier = slowMultiplier;
        m_SlowTime = slowTime;
    }

}