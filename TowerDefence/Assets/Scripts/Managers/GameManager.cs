using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Collections;
using UnityEngine.EventSystems;

public enum GameState {
    beforeGame,
    inGame,
    paused,
    afterGame
}

/// <summary>
/// Handles the GameState. 
/// </summary>
public class GameManager : MonoBehaviour {

    #region Fields

    public bool devMode;

    public UnityEvent StartGameEvents;

    private bool isTimerOn = true;
    public float elapsedTime { get; private set; } = 0;
    private int enemiesKilled = 0; // we can maybe use this in a later episode ^^

    /// <summary>
    /// Amount of lives currently left.
    /// </summary>
    private int lives {
        get => Lives;
        set { Lives = value < 0 ? 0 : value; }
    }
    private int Lives = Config.LIVES;

    /// <summary>
    /// How many waves has the player survived.
    /// </summary>
    public int WavesSurvived { get; private set; } = 0;

    /// <summary>
    /// Number of current wave, 1 during first wave...
    /// </summary>
    public int currentWave { get; private set; } = 0;

    /// <summary>
    /// GameState that the game is currently in.
    /// </summary>
    public GameState currentGameState { get; private set; }

    /// <summary>
    /// Is <see cref="currentGameState"/> currently in inGame.
    /// </summary>
    public bool isInGame { get => currentGameState == GameState.inGame; }

    /// <summary>
    /// Percentage of increase in wave survival bonus if wave skipped.
    /// </summary>
    public int currentWaveReward { get => (int)(((Config.TIME_BETWEEN_WAVES - GameManager.instance.elapsedTime) / Config.TIME_BETWEEN_WAVES) * 100) / 10 * 10 / 2; } // All of this is nessecary! No one really thought that someone might want to round things to nearest 10/5

    public static GameManager instance;


    #endregion
    #region Unity

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[GameManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }

    }


    private void Start() {
        currentGameState = GameState.beforeGame;

        // Initialize random
        Seed.instance.CreateSeed();

        AI.waveGenerator.SetRandom(Seed.instance.seed);
        Random.InitState(Seed.instance.seed);

        // Initialise UI elements
        UIManager.instance.UpdateTextElement("currency", Currency.amount.ToString());
        UIManager.instance.UpdateTextElement("waves", currentWave.ToString());
        UIManager.instance.UpdateTextElement("lives", lives.ToString());
        UIManager.instance.SetTowerCosts();

        // Skip "mainmenu" when replaying seed
        if (Seed.instance.shouldStartGameOnLoad) StartCoroutine(StartGameAfterDelay());
    }


    private void Update() {
        // Listen input related to current GameState
        InputManager.instance.ListenInput(currentGameState);

        // Invoke GameState related actions here
        switch (currentGameState) {
            case GameState.beforeGame:
                break;
            case GameState.inGame:
                Game();
                break;
            case GameState.paused:
                break;
            case GameState.afterGame:
                break;
        }
    }


    #endregion
    #region Private methods


    /// <summary>
    /// Start game after all Start functions have run.
    /// <summary>
    private IEnumerator StartGameAfterDelay() {
        yield return new WaitForEndOfFrame();
        ReferencesUI.instance.pregameGUI.SetActive(false);
        StartGame();
    }


    /// <summary>
    /// Runs the game. Invoked from Update() when GameState is inGame.
    /// </summary>
    private void Game() {
        // Countdown for next wave
        if (isTimerOn) {
            elapsedTime += Time.deltaTime;
            UIManager.instance.UpdateTimer(elapsedTime);
        }

        // Start new wave
        if (elapsedTime >= Config.TIME_BETWEEN_WAVES)
            AI.instance.StartWave();
    }


    /// <summary>
    /// Function for ending the game depending on paramter
    /// </summary>
    private void GameOver() {
        lives = 0; // Update lives one last time
        UIManager.instance.UpdateTextElement("lives", lives.ToString());
        currentGameState = GameState.afterGame;
        BuildManager.instance.DeselectTurret();
        Pause.instance.TryUnPause();
        TimeManager.instance.ApplyTimeScale(0f);
        UIManager.instance.OnGameOver();
    }


    #endregion
    #region Public methdods


    /// <summary>
    /// Start game, triggers events and sets game started to true
    /// </summary>
    public void StartGame() {
        if (isInGame) return;
        currentGameState = GameState.inGame;
        EventSystem.current.SetSelectedGameObject(null);
        StartGameEvents.Invoke(); // Invoke events, button settings etc
        EnemyPools.instance.Configure(); // Generate enemy pools
        GridManager.instance.CreatePathColliders(); // Generate colliders for raycast configuring
        Seed.instance.shouldStartGameOnLoad = false;
    }


    /// <summary>
    /// Reloads the scene
    /// </summary>
    public void RestartGame() {
        Scene currentscene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentscene.name);
    }


    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    /// <summary>
    /// Toggle GameState between ingame and paused.
    /// </summary>
    /// <returns> true: GameState is paused after | false: GameState is not paused after</returns>
    public bool TogglePause() {
        // Toggle pause if GameState is inGame/paused, ignore if in other GameState
        if (currentGameState == GameState.inGame)
            currentGameState = GameState.paused;
        else if (currentGameState == GameState.paused)
            currentGameState = GameState.inGame;

        EventSystem.current.SetSelectedGameObject(null);

        return currentGameState == GameState.paused;
    }


    /// <summary>
    /// Start the timer for the wavedelay.
    /// </summary>
    public void StartTimer() {
        isTimerOn = true;
        ReferencesUI.instance.waveTimer.enabled = true;
    }


    /// <summary>
    /// Reloads the scene with same random seed
    /// </summary>
    public void LoadSceneWithSameSeed() {
        // Set current seed to custom seed so that it will be used in seed.initstate
        Seed.instance.customSeed = Seed.instance.seed;
        // when this is true, we use the seed we already had and go straight tot he game
        Seed.instance.shouldStartGameOnLoad = true;
        RestartGame();
    }


    /// <summary>
    /// Handle enemy kill.
    /// </summary>
    /// <remarks><param name="enemyCode">Code corresponding the enemy killed</param></remarks>
    public void OnEnemyKill(int enemyCode, int waveIndex) {
        AI.instance.OnEnemyKill(waveIndex);
        enemiesKilled++;
        Currency.Use(Enemies.all[enemyCode].reward);
    }


    /// <summary>
    /// Handle enemy which got trough to the end.
    /// </summary>
    public void OnEnemyGotTrough(int waveIndex) {
        if (--lives == 0) {
            GameOver();
            return;
        }

        // Update GUI and notify AI
        UIManager.instance.UpdateTextElement("lives", lives.ToString());
        AI.instance.OnEnemyGotTrough(waveIndex);
    }


    /// <summary>
    /// Handle wave survived.
    /// </summary>
    public void OnWaveSurvived(Wave _Wave) {
        WavesSurvived++;

        // If reward, give it and enqueue notification
        if (_Wave.reward == 0) return;
        Currency.Use(_Wave.reward);
        UserNotification.instance.QueueNotification(NotificationCode.waveSurvived, (_Wave.index, _Wave.reward));
    }


    /// <summary>
    /// Handle new wave start.
    /// </summary>
    /// <param name="waveStartedEarly">If wave was started early</param>
    public void OnWaveStart() {
        currentWave++;
        isTimerOn = false;
        ReferencesUI.instance.waveTimer.enabled = false;
        elapsedTime = 0f;

        UIManager.instance.UpdateTextElement("waves", currentWave.ToString());
    }

    #endregion
}