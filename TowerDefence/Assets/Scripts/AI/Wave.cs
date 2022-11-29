using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Handle wave data.
/// </summary>
public class Wave {

    /// <summary>
    /// Enemy stacks of wave. 
    /// </summary>
    public List<EnemyGroup> enemyGroups { get; private set; }

    /// <summary>
    /// Amount of enemies alive belonging to the wave.
    /// </summary>
    public int enemyCount;

    /// <summary>
    /// Reward for survining wave.
    /// </summary>
    public int reward { get; set; } = 0;

    /// <summary>
    /// index of wave.
    /// </summary>
    public int index { get; private set; }

    /// <summary>
    /// Have player survived the wave.
    /// </summary>
    public bool survived { get => enemyCount == 0; }


    /// <summary>
    /// Add group of enemies belongin to the wave to <see cref="enemyGroups"/>.
    /// </summary>
    public void AddGroup(int enemy, float delay1, int amount, float delay2) {
        enemyCount += amount;
        enemyGroups.Add(new EnemyGroup(enemy, delay1, amount, delay2));
    }


    public Wave(int id) {
        this.index = id;
        this.enemyGroups = new List<EnemyGroup>();
    }


    public Wave(int id, int reward, List<EnemyGroup> enemyStacks) {
        this.index = id;
        this.reward = reward;
        this.enemyGroups = enemyStacks;
        this.enemyCount = enemyStacks.Sum(e => e.amount);
    }
}


/// <summary>
/// Enemy stack.
/// </summary>
public struct EnemyGroup {

    /// <summary>
    /// Code of enemy to be spawned from stack.
    /// </summary>
    public int enemy { get; }

    /// <summary>
    /// Delay in seconds between spawning enemies from stack.
    /// </summary>
    public float delayBetween { get; }

    /// <summary>
    /// Amount of enemies to be spawned from stack.
    /// </summary>
    public int amount { get; }

    /// <summary>
    /// Delay in seconds until advancing to next stack.
    /// </summary>
    public float delayToNext { get; }

    public EnemyGroup(int enemy, float delayBetween, int amount, float delayToNext) {
        this.enemy = enemy;
        this.delayBetween = delayBetween;
        this.amount = amount;
        this.delayToNext = delayToNext;
    }
}