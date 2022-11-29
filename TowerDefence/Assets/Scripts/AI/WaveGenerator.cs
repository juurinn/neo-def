using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Generate random waves from set seed.
/// </summary>
public class WaveGenerator {

    /// <summary>
    ///
    /// </summary>
    private Random random;

    /// <summary>
    /// Index of bosswave last spawned.
    /// </summary>
    private int bossWaveNumber;

    /// <summary>
    /// Number of current wave.
    /// </summary>
    private int waveNumber { get => GameManager.instance.currentWave; }

    /// <summary>
    /// Delay on spawns between invidual enemies.
    /// </summary>
    private float delayBetweenEnemies {
        get => RandomFromRange(WaveConfig.MIN_DELAY_BETWEEN_ENEMIES, WaveConfig.MAX_DELAY_BETWEEN_ENEMIES);
    }

    /// <summary>
    /// Delay on spawns between groups.
    /// </summary>
    private float delayBetwenGroups {
        get => RandomFromRange(WaveConfig.MIN_DELAY_BETWEEN_GROUP, WaveConfig.MAX_DELAY_BETWEEN_GROUP);
    }

    /// <summary>
    /// Current budget for spawning enemies.
    /// </summary>
    private int currentBudget {
        get => Math.Min(WaveConfig.BASE_WAVE_BUDGET + (waveNumber - 1) * WaveConfig.WAVE_BUDGET_INC, WaveConfig.WAVE_BUDGET_CAP);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float RandomFromRange(float min, float max) =>
        (float)random.NextDouble() * (min - max) + min;


    /// <summary>
    /// Create and store random from seed.
    /// </summary>
    public void SetRandom(int seed) =>
        random = new Random(seed);


    /// <summary>
    /// Get wave.
    /// </summary>
    /// <param name="isBoss">Should return boss wave.</param>
    /// <returns>Generated wave.</returns>
    public Wave Get(bool isBoss) => isBoss ? Boss() : Normal();


    /// <summary>
    /// Return harcoded bosswave.
    /// </summary>
    private Wave Boss() {
        // Get correct bosswave
        (int, List<EnemyGroup>) bossWave = WaveConfig.bossWaves[bossWaveNumber];

        if(bossWaveNumber < WaveConfig.bossWaves.Length - 1) bossWaveNumber++;
        return new Wave(waveNumber, bossWave.Item1, bossWave.Item2);
    }


    /// <summary>
    /// Generate wave.
    /// </summary>
    /// <returns>Generated wave.</returns>
    private Wave Normal() {
        // Create wave
        Wave wave = new Wave(waveNumber);
        int tempBudget = currentBudget;

        // Get enemypool of wave
        var enemyPool = Enemies.all.Take(WaveConfig.GetWaveEnemyPool(waveNumber) + 1);
        int minGroupSize = Math.Min(7, 2 + bossWaveNumber * 2);
        int maxGroupSize = Math.Min(12, minGroupSize + 2 + bossWaveNumber * 2);

        // Fill wave with enemies
        while (true) {
            // Get random enemy
            var enemy = GetRandomEnemy(tempBudget);
            if (enemy == null) break; // Couldn't afford enemy, budget is fully used

            // Add group of enemy to wave and spend budget
            tempBudget -= AddStack(enemy.Value.Item1, enemy.Value.Item2);
        };

        // Set wave reward
        wave.reward = wave.enemyCount * Math.Min(Math.Max(5, 4 + (int)Math.Pow(2.35f, bossWaveNumber)), 65);

        // Add bosses to wave
        if (waveNumber > 60)
            AddBosses();

        // Return results
        return wave;

        (int, int)? GetRandomEnemy(int budget) {
            // Get max enemy index from config
            var enemies = enemyPool.Where(e => e.weight <= budget).Select(e => (e.index, e.weight)).ToArray();

            // Return null if budget is less than any enemy, else return random enemy with lower cost than budget
            return enemies.Count() == 0 ? null : ((int, int)?)enemies[random.Next(0, enemies.Count())];
        }

        int AddStack(int enemy, int cost) {
            // Get random amount and limit it with budget.
            int amount = Math.Min((int)(tempBudget / cost), random.Next(minGroupSize, maxGroupSize));

            // Add stack to wave
            wave.AddGroup(enemy, 1f / Enemies.all[enemy].speed * delayBetweenEnemies, amount, delayBetwenGroups);

            // Return amount of budget used.
            return amount * cost;
        }

        void AddBosses() {
            // Amount of bosses
            int amount = waveNumber >= 100 ? random.Next(3, 6) : random.Next(0, 3);

            // Max Index
            int max = wave.enemyGroups.Count;

            while (amount > 0) {
                // Get random boss from pool
                int boss = random.Next(7, WaveConfig.GetWaveBossPool(waveNumber));

                // Add stack to random index
                wave.enemyGroups.Insert(random.Next(0, max - 1), new EnemyGroup(boss, 0, 1, delayBetwenGroups));
                wave.enemyCount++;

                max++; amount--;
            }
        }
    }
}