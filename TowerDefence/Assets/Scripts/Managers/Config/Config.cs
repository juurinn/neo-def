using UnityEngine;

/// <summary>
/// Handle configure data.
/// </summary>
public class Config : MonoBehaviour {

    private Config instance;

    private void Awake() {
        // There should be only one config in scene
        if (instance != null) {
            Debug.LogError("[Config]: Multiple instances, this instance at " + gameObject);
            Destroy(gameObject);
        } else instance = this;

        // Assign hardcoded values to structs
        TurretConfig.Initialise();
    }


    #region Options

    /// <summary>
    /// Default audio volume.
    /// </summary>
    public const float DEFAULT_AUDIO_VOLUME = 0.25f;

    /// <summary>
    /// Default display mode.
    /// </summary>
    public const int DEFAULT_DISPLAY_MODE = 0; // Fullscreen windowed

    /// <summary>
    /// Default resolution.
    /// </summary>
    public const int DEFAULT_RESOLUTION = 2; // Full HD


    #endregion
    #region GameSettings


    /// <summary>
    /// Amount lives player has at start.
    /// </summary>
    public const int LIVES = 50;

    /// <summary>
    /// Delay between waves in seconds.
    /// </summary>
    public const int TIME_BETWEEN_WAVES = 30; // Should have enough time to let player think / read upgrades etc

    /// <summary>
    /// Max amount of waves running simultaneously.
    /// </summary>
    public const int MAX_WAVES_RUNNING = 20; // int.MaxValue


    #endregion
    #region Currency


    /// <summary>
    /// Amount of currency at start.
    /// </summary>
    public const int BASE_CURRENCY = 2000;

    /// <summary>
    /// Percentage of turret cost given back on turret sell.
    /// </summary>
    public const float SELL_MULTIPLIER = 0.75f;

    /// <summary>
    ///  How fast wave skip reward should increase.
    /// </summary>
    public const float WAVE_SKIP_MULTIPLIER = 0.5f;


    #endregion
    #region TileCodes


    public const int TILE_CODE_GROUND = 0;

    public const int TILE_CODE_PATH = 1;

    public const int TILE_CODE_HILL = 2;

    public const int TILE_CODE_TOWER = 3;

    #endregion


    public static string NotificationMessage(NotificationCode _Notification, (int, int)? waveData = null) {
        switch (_Notification) {
            case NotificationCode.waveSurvived:
                return "Cleared wave: " + waveData.Value.Item1 + " reward: " + waveData.Value.Item2;
            case NotificationCode.bossWave:
                return "Boss Wave incoming! Clear the wave for great reward!";
            case NotificationCode.notEnoughCoffee:
                return "DEVS RAN OUT OF COFFEE. PLEASE HELP";
            default:
                return "Error";
        }
    }


    public static string ErrorMessage(ErrorMsgCode _Error) {
        switch (_Error) {
            case ErrorMsgCode.cantAffordTower:
                return "YOU DO NOT HAVE ENOUGH CURRENCY TO BUY THIS TOWER";
            case ErrorMsgCode.cantAffordUpgrade:
                return "YOU DO NOT HAVE ENOUGH CURRENCY TO BUY THIS UPGRADE";
            case ErrorMsgCode.towerOnPathErr:
                return "TOWER CANNOT BE PLACED THERE";
            case ErrorMsgCode.towerOutsideMapErr:
                return "TOWER CANNOT BE PLACED OUTISIDE OF THE MAP";
            case ErrorMsgCode.notEnoughCoffee:
                return "DEVS RAN OUT OF COFFEE. PLEASE HELP";
            default:
                return "Error";
        }
    }
}


public enum NotificationCode {
    waveSurvived,
    bossWave,
    notEnoughCoffee
}


public enum ErrorMsgCode {
    cantAffordTower,
    cantAffordUpgrade,
    towerOnPathErr,
    towerOutsideMapErr,
    notEnoughCoffee
}