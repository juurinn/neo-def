using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// <para> Handles inputs depending on game state </para>
/// <para> Upgrading to Unity's new input system would be a better solution for this </para>
/// </summary>
public class InputManager : MonoBehaviour {

    public static InputManager instance;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[InputManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// Listen user input.
    /// </summary>
    /// <param name="_GameState">Current state of the game.</param>
    public void ListenInput(GameState _GameState) {
        switch (_GameState) {
            case GameState.beforeGame:
                PreGameInputs();
                break;
            case GameState.inGame:
                GameInputs();
                break;
            case GameState.afterGame:
                AfterGameInputs();
                break;
            case GameState.paused:
                PauseInputs();
                break;
            default:
                Debug.Log("ERROR: No gamestate defined");
                break;
        }

        if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.F10))
            GameManager.instance.devMode = true;
    }


    /// <summary>
    /// Listen inputs on map seletion and executes functions from GameManager instance
    /// </summary>
    private void PreGameInputs() {
        // New map
        if (Input.GetKeyDown(KeyCode.G))
            GameManager.instance.RestartGame();
        // Start game
        else if (Input.GetKeyDown(KeyCode.S))
            GameManager.instance.StartGame();
    }


    /// <summary>
    /// Handle inputs during game.
    /// </summary>
    private void GameInputs() {
        // New wave
        if (Input.GetKeyDown(KeyCode.Space))
            AI.instance.StartWave(GameManager.instance.currentWaveReward);
        // TimeScale
        else if (Input.GetKeyDown(KeyCode.Tab))
            TimeManager.instance.ToggleTimeScale();
        // Closing/Pausing
        else if (Input.GetKeyDown(KeyCode.Escape))
            HandleESC();
        // Dev Restart
        else if (Input.GetKeyDown(KeyCode.G) && GameManager.instance.devMode)
            GameManager.instance.RestartGame();

        // Sell tower
        else if (Input.GetKeyDown(KeyCode.S))
            TowerManager.instance.SellTower();
        // Upgrade left
        else if (Input.GetKeyDown(KeyCode.Q))
            TowerManager.instance.UpgradeTower(1, true);
        // Single upgrades
        else if (Input.GetKeyDown(KeyCode.W))
            TowerManager.instance.UpgradeTower(2);
        // Upgrade right
        else if (Input.GetKeyDown(KeyCode.E))
            TowerManager.instance.UpgradeTower(2, true);
        // TargetMode to left
        else if (Input.GetKeyDown(KeyCode.A))
            TowerManager.instance.ChangeTargetingMode(-1);
        // TargetMode to right
        else if (Input.GetKeyDown(KeyCode.D))
            TowerManager.instance.ChangeTargetingMode(1);

        // Mouse left
        else if (Input.GetMouseButtonDown(0))
            HandleLeftMouse();
        // Mouse right
        else if (Input.GetMouseButtonDown(1))
            HandleRight();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Shop.instance.SelectTurret(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            Shop.instance.SelectTurret(3);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            Shop.instance.SelectTurret(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            Shop.instance.SelectTurret(4);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            Shop.instance.SelectTurret(1);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            Shop.instance.SelectTurret(5);
    }


    /// <summary>
    /// Handle inputs after game.
    /// </summary>
    private void AfterGameInputs() {
        if (Input.GetKeyDown(KeyCode.R))
            GameManager.instance.RestartGame();
    }


    /// <summary>
    /// Handle inputs during pause.
    /// </summary>
    private void PauseInputs() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Pause.instance.TryUnPause();
        }
    }


    /// <summary>
    /// Handle left mousebutton, inGame.
    /// </summary>
    private void HandleLeftMouse() {
        // If in building in progress
        if (BuildManager.instance.CanBuild) {
            // Place turret
            BuildManager.instance.TryPlaceTurret();
            // DeSelect turret if shift not pressed
            if (!Input.GetKey(KeyCode.LeftShift))
                BuildManager.instance.DeselectTurret();
            return;
        }

        // If tower selected
        if (TowerManager.instance.IsTowerSelected) {
            if (!EventSystem.current.IsPointerOverGameObject())
                TowerManager.instance.TrySelectTower();
            return;
        }

        // Try selecting tower
        TowerManager.instance.TrySelectTower();
    }


    /// <summary>
    /// Handle left mousebutton, inGame.
    /// </summary>
    private void HandleRight() {
        BuildManager.instance.DeselectTurret();
        TowerManager.instance.DeSelectTower();
    }



    /// <summary>
    /// Handle Esc inGame.
    /// </summary>
    private void HandleESC() {
        // Close if something to close
        if (BuildManager.instance.CanBuild || TowerManager.instance.IsTowerSelected)
            CloseAll();
        // Pause
        else
            Pause.instance.TryPause();
    }


    /// <summary>
    /// Close all interfaces
    /// </summary>    
    private void CloseAll() {
        BuildManager.instance.DeselectTurret();
        TowerManager.instance.DeSelectTower();
        UIManager.instance.DisableTowerUpgradeButtons();
    }
}