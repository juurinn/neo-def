using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages spawning of waves and enemies
/// </summary>
public class AI : MonoBehaviour {

    public static AI instance;
    public static WaveGenerator waveGenerator;

    /// <summary>
    /// Waves that have enemies alive currently.
    /// </summary>
    private List<Wave> wavesRunning = new List<Wave>();
    private int amountOfWavesRunning { get => wavesRunning.Count; }

    /// <summary>
    /// Amount of waves currently in process of spawning.
    /// </summary>
    private int wavesBeingSpawned;

    /// <summary>
    /// Is current wave a bosswave
    /// </summary>
    private bool isBossWave { get => GameManager.instance.currentWave % 10 == 0; }

    /// <summary>
    /// Path for enemies to travel, coordinates of corners.
    /// </summary>
    public List<Vector2> path { get; private set; }


    private void Awake() {
        if (instance == null) {
            instance = this;
            waveGenerator = new WaveGenerator();
        } else {
            Debug.LogError("[AI]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Starts wave.
    /// </summary>
    public void StartWave(float bonusPercentage = 0) {
        // Enable/Disable NextWaveButton depending of if max limit reached
        UIManager.instance.SetNextWaveButton(amountOfWavesRunning + 1);

        if (amountOfWavesRunning >= Config.MAX_WAVES_RUNNING) {
            Debug.LogWarning("[GameManager]: Trying to start wave but max waves running simultaneously is already met");
            return;
        }

        // Notify gamemanager about wave being started
        GameManager.instance.OnWaveStart();

        // Get wave to be spawned
        Wave wave = waveGenerator.Get(isBossWave);

        // If bonusPercentage not 0, increase reward by percentage
        if (bonusPercentage != 0)
            wave.reward = wave.reward + (int)(wave.reward * bonusPercentage / 100f);

        // Start spawning the wave
        StartCoroutine(SpawnWave(wave));
    }


    /// <summary>
    /// Handle enemy which made it to the end.
    /// </summary>
    /// <param name="_Index">Wave index of enemy.</param>
    public void OnEnemyGotTrough(int _Index) {
        // Get wave of enemy
        Wave _Wave = GetWaveByIndex(_Index);

        // Makes sure that reward is nullified
        if (_Wave.reward != 0) _Wave.reward = 0;

        HandleWaveSurvival(_Wave);
    }


    /// <summary>
    /// Handle enemy kill.
    /// </summary>
    /// <param name="_Index">Wave index of enemy.</param>
    public void OnEnemyKill(int _Index) {
        // Get wave of enemy
        Wave _Wave = GetWaveByIndex(_Index);

        HandleWaveSurvival(_Wave);
    }


    /// <summary>
    /// Get <see cref="Wave"/> corresponding index.
    /// </summary>
    /// <param name="_Index">Index of wave.</param>
    /// <returns>If not found: <see langword="null"/> | Else: <see cref="Wave"/></returns>
    private Wave GetWaveByIndex(int _Index) {
        Wave _Wave = wavesRunning.Find(w => w.index == _Index);

        if (_Wave == null) {
            Debug.LogWarning($"[AI] Couldn't find wave with index: {_Index}");
            return null;
        }

        return _Wave;
    }


    /// <summary>
    /// Handle wave survival.
    /// </summary>
    /// <param name="_Wave">Wave to be handled.</param>
    private void HandleWaveSurvival(Wave _Wave) {
        _Wave.enemyCount -= 1;
        if (!_Wave.survived) return;

        // Handle wave survival
        wavesRunning.Remove(_Wave);
        UIManager.instance.SetNextWaveButton(amountOfWavesRunning - 1);

        GameManager.instance.OnWaveSurvived(_Wave);
    }


    /// <summary>
    /// Handle spawning of wave.
    /// </summary>
    /// <param>Wave to be spawned</param>
    private IEnumerator SpawnWave(Wave _Wave) {
        wavesBeingSpawned++;
        wavesRunning.Add(_Wave);

        // Loop trough all enemy stacks in the wave
        foreach (EnemyGroup enemyStack in _Wave.enemyGroups) {
            // Spawn all enemies from stack
            for (int i = 0; i < enemyStack.amount; i++) {
                SpawnEnemy(enemyStack.enemy, _Wave.index);
                // Wait delay between spawns
                yield return new WaitForSeconds(enemyStack.delayBetween);
            }
            // Wait delay before next stack
            yield return new WaitForSeconds(enemyStack.delayToNext);
        }

        OnWaveSpawned();
    }


    /// <summary>
    /// Spawns an individual enemy unit from pool
    /// </summary>
    private void SpawnEnemy(int m_EnemyIndex, int _WaveIndex) {
        GameObject enemy = EnemyPools.instance.GetGameObjectFromPool(m_EnemyIndex);
        enemy.transform.position = new Vector3(path[0].x, path[0].y, 0);
        enemy.SetActive(true);

        enemy.GetComponent<Enemy>().Enable(_WaveIndex);
    }


    /// <summary>
    /// Handle wave been fully spawned. 
    /// </summary>
    private void OnWaveSpawned() {
        wavesBeingSpawned--;

        // Start countdown for next if last wave being spawned
        if (wavesBeingSpawned == 0) GameManager.instance.StartTimer();
    }


    /// <summary>
    /// Stores path given as spots to Vector2's
    /// </summary>
    /// <param name="spots"> List of coordinates as spots </param>
    public void StorePath(List<Spot> spots) {
        path = new List<Vector2>();
        foreach (var spot in spots)
            path.Add(new Vector2(spot.x + GridManager.instance.roadMap.tileAnchor.x, spot.y + GridManager.instance.roadMap.tileAnchor.y));
    }


    private int pekkaCounter;
    public void Update() {
        if (GameManager.instance.devMode) {
            if (Input.GetKeyDown(KeyCode.F7)) { StartCoroutine(SpawnWave(new Wave(999, 10, WaveConfig.bossWaves[pekkaCounter].Item2))); print("[AI.Dev]: started bosswave"); }
            if (Input.GetKeyDown(KeyCode.F8)) { pekkaCounter++; print("[AI.Dev]: Pekkas ego is at level " + pekkaCounter + " now!"); }
            if (Input.GetKeyDown(KeyCode.F6)) { GameManager.instance.OnWaveStart(); print("[AI.Dev]: skipped wave"); }
        }
    }
}
