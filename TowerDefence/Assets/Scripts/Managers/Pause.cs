using UnityEngine;

/// <summary>
/// Handles pausing and unpausing the game.
/// </summary>
public class Pause : MonoBehaviour {

    static public Pause instance;

    [SerializeField] private GameObject pausePanel;

    /// <summary>
    /// Is the game currently paused.
    /// </summary>
    public bool IsGamePaused {
        get => GameManager.instance.currentGameState == GameState.paused;
    }
    
    private float m_TimeBeforePause;


    private void Awake() {
        if (instance == null) {
            instance = this;
            Initialization();
        } else {
            Debug.LogError("[TimeManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Initializes pause.
    /// </summary>
    private void Initialization() {
        // If pause UI is active, disable it
        if (pausePanel.activeInHierarchy) {
            pausePanel.SetActive(false);
        }
    }


    /// <summary>
    /// Checks if we are allowed to pause the game.
    /// </summary>
    /// <returns> true/false depending if we can pause. </returns>
    private bool CanPause() {
        bool b;
        if (GameManager.instance.isInGame) {
            b = true;
        } else {
            b = false;
        }
        return b;
    }


    /// <summary>
    /// Tries to pause the game if all conditions are met.
    /// </summary>
    /// <returns> true: on success | false: on fail </returns>
    public bool TryPause() {
        if (CanPause()) {
            if (!IsGamePaused) {
                PauseGame();
                return true;
            } else {
                return false;
            }
        } else {
            // Exit out of other things, if we even have any
            return false;
        }
    }

    /// <summary>
    /// Tries to unpause the game.
    /// </summary>
    /// <returns> true: on success | false: on fail </returns>
    public bool TryUnPause() {
        // TODO: Close pause menus
        if (IsGamePaused) {
            UnPauseGame();
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGame() {
        m_TimeBeforePause = TimeManager.instance.CurrentTimeScale;
        TimeManager.instance.ApplyTimeScale(0f);
        pausePanel.SetActive(true);
        GameManager.instance.TogglePause();
    }

    /// <summary>
    /// Unpauses the game.
    /// </summary>
    private void UnPauseGame() {
        TimeManager.instance.ApplyTimeScale(m_TimeBeforePause);
        pausePanel.SetActive(false);
        GameManager.instance.TogglePause();
    }


    /// <summary>
    /// This is for pause menu continue button.
    /// </summary>
    public void Continue() {
        TryUnPause();
    }

}
