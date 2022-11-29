using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data of bosswaves and for generating waves.
/// </summary>
public class WaveConfig : MonoBehaviour {

    /// <summary>
    /// Multiplier for hp at every 10th level, starting from wave 11.
    /// </summary>
    public const float HP_INCREASE_MULTIPLIER = 2f;

    /// <summary>
    /// Starting budget.
    /// </summary>
    public const int BASE_WAVE_BUDGET = 250;

    /// <summary>
    /// Increase in budget after every wave.
    /// </summary>
    public const int WAVE_BUDGET_INC = 50;

    /// <summary>
    /// Maximum wave budget. Atm hits the cap around wave 40.
    /// </summary>
    public const int WAVE_BUDGET_CAP = 2200;

    /// <summary>
    /// Minimum time in seconds between enemies while spawning.
    /// </summary>
    public const float MIN_DELAY_BETWEEN_ENEMIES = 0.7f;

    /// <summary>
    /// Maximum time in seconds between enemies while spawning.
    /// </summary>
    public const float MAX_DELAY_BETWEEN_ENEMIES = 1f;

    /// <summary>
    /// Minimum time in seconds between enemy groups while spawning.
    /// </summary>
    public const float MIN_DELAY_BETWEEN_GROUP = 0.5f;

    /// <summary>
    /// Maximum time in seconds between enemy groups while spawning.
    /// </summary>
    public const float MAX_DELAY_BETWEEN_GROUP = 1f;


    private static int normal = Enemies.Normal.index;
    private static int slow = Enemies.Slow.index;
    private static int fast = Enemies.Fast.index;
    private static int unslowable = Enemies.Unslowable.index;
    private static int healer = Enemies.Healer.index;
    private static int sprinter = Enemies.Sprinter.index;
    private static int superFast = Enemies.SuperFast.index;
    private static int nBoss = Enemies.NormalBoss.index;
    private static int fboss = Enemies.FastBoss.index;
    private static int hBoss = Enemies.HealerBoss.index;
    private static int uBoss = Enemies.UnslowableBoss.index;
    private static int sBoss = Enemies.SprinterBoss.index;



    /// <summary>
    /// Enemies and rewads of bosswaves. 
    /// </summary>
    public static (int, List<EnemyGroup>)[] bossWaves = new (int, List<EnemyGroup>)[] {
        //1
        (700,
        new List<EnemyGroup> {
            new EnemyGroup(nBoss, 0.3f, 1, 1f),
            new EnemyGroup(normal, 0.2f, 8, 1f),
            new EnemyGroup(normal,0.2f, 8, 5f),
        }),
        //2
        (500,
        new List<EnemyGroup>{
           new EnemyGroup(normal,0.2f, 15, 2f),
           new EnemyGroup(fboss,0.5f, 2, 1f),
           new EnemyGroup(fast,0.3f, 15, 5f),
        }),
        //3
        (3000,
        new List<EnemyGroup> {
            new EnemyGroup(slow, 0.2f, 6, 1.5f),
            new EnemyGroup(hBoss, 1f, 1, 1f),
            new EnemyGroup(hBoss, 1f, 1, 0.1f),
            new EnemyGroup(normal,0.2f, 10, 1f),
            new EnemyGroup(hBoss, 1f, 1, 2f),
            new EnemyGroup(fast,0.2f, 5, 1f),
            new EnemyGroup(fast,0.2f, 5, 1f),
            new EnemyGroup(fast,0.2f, 5, 1f),
            new EnemyGroup(fast,0.2f, 5, 5f),
        }),
        //4
        (5500,
        new List<EnemyGroup> {
            new EnemyGroup(normal,0.2f, 10, 1f),
            new EnemyGroup(sBoss, 0.5f, 1, 2f),
            new EnemyGroup(healer,0.2f, 6, 0f),
            new EnemyGroup(sBoss, 0.5f, 1, 2f),
            new EnemyGroup(normal,0.2f, 10, 1f),
            new EnemyGroup(sBoss, 0.5f, 1, 10f),
        }),
        //5
        (5000,
        new List<EnemyGroup> {
            new EnemyGroup(uBoss, 2f, 15, 6f),
        }),
        //6
        (60000,
        new List<EnemyGroup> {
            new EnemyGroup(Enemies.Pekka.index, 0, 1, 15f),
        }),
        //7
        (100000,
        new List<EnemyGroup> {
            new EnemyGroup(uBoss, 0.5f, 3, 3f),
            new EnemyGroup(superFast, 0.1f, 15, 0),
            new EnemyGroup(hBoss, 0, 1, 3f),
            new EnemyGroup(slow, 0.3f, 7, 1.5f),
            new EnemyGroup(normal, 0.5f, 7, 1.5f),
            new EnemyGroup(normal, 0.5f, 7, 1.5f),
            new EnemyGroup(fboss, 0.5f, 2, 1f),
        }),
        //8
        (150000,
        new List<EnemyGroup> {
            new EnemyGroup(slow, 0.5f, 5, 2f),
            new EnemyGroup(normal, 0.2f, 8, 2f),
            new EnemyGroup(healer, 0.2f, 10, 1f),
            new EnemyGroup(fboss, 0.2f, 10, 3f),
            new EnemyGroup(superFast, 0.1f, 15, 0),
        }),
        //9
        (250000,
        new List<EnemyGroup> {
            new EnemyGroup(nBoss, 0.3f, 10, 1f),
            new EnemyGroup(fboss, 0.3f, 10, 5f),
            new EnemyGroup(fast, 0.3f, 50, 20f)
        }),
        //10
        (500000,
        new List<EnemyGroup> {
            new EnemyGroup(nBoss, 0.3f, 10, 1f),
            new EnemyGroup(fboss, 0.3f, 10, 5f),
            new EnemyGroup(normal, 0.3f, 25, 20f)
        }),
        //11
        (750000,
        new List<EnemyGroup> {
            new EnemyGroup(nBoss, 1f, 2, 2f),
            new EnemyGroup(fboss, 1f, 2, 2f),
            new EnemyGroup(hBoss, 1f, 2, 2f),
            new EnemyGroup(uBoss, 1f, 2, 2f),
            new EnemyGroup(sBoss, 1f, 2, 2f),
            new EnemyGroup(nBoss, 1f, 2, 20f),
        }),
    };

    /// <summary>
    /// Get enemy pool for wave.
    /// </summary>
    public static int GetWaveEnemyPool(int wave) =>
        Mathf.FloorToInt(Mathf.Min(wave / 7, 6));


    /// <summary>
    /// Get boss pool for wave.
    /// </summary>
    public static int GetWaveBossPool(int wave) =>
        Mathf.FloorToInt(Mathf.Min(Mathf.Max(wave / 7, 7), 11));


    /// <summary>
    /// Double hp for every 10th wave starting from wave 11.
    /// </summary>
    public static float HpMultiplier(int enemy, int wIndex) =>
        Enemies.all[enemy].hp * Mathf.Min(Mathf.Pow(WaveConfig.HP_INCREASE_MULTIPLIER, Mathf.Ceil((wIndex - 1) / 10)), 1024);
}
