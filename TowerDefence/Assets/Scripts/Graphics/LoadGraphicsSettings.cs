using UnityEngine;

/// <summary>
/// Handles loading and applying graphics settings.
/// </summary>
public static class LoadGraphicsSettings {

    // Access to the values
    public static int DisplayMode { get; set; }
    public static int Resolution { get; set; }

    // Runtime values
    private static int m_ResolutionX;
    private static int m_ResolutionY;
    private static FullScreenMode m_DisplayMode;

    /// <summary>
    /// Loads values from player prefs and applies them before game starts.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void LoadSavedValues() {
        DisplayMode = PlayerPrefs.GetInt("DisplayMode", Config.DEFAULT_DISPLAY_MODE);
        SetDisplayMode();
        Resolution = PlayerPrefs.GetInt("Resolution", Config.DEFAULT_RESOLUTION);
        SetResolution();
        ApplyAll();
    }

    /// <summary>
    /// Sets display mode value according to the dropdown value.
    /// </summary>
    /// <param name="applyValues"> Do we also apply set value. </param>
    public static void SetDisplayMode(bool applyValues = false) {
        switch (DisplayMode) {
            case 0:
                m_DisplayMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                m_DisplayMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                m_DisplayMode = FullScreenMode.Windowed;
                break;
            default:
                m_DisplayMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
        if (applyValues) ApplyAll();
    }

    /// <summary>
    /// Sets resolution value according to the dropdown value.
    /// </summary>
    /// <param name="applyValues"> Do we also apply set value. </param>
    public static void SetResolution(bool applyValues = false) {
        switch (Resolution) {
            case 0:
                m_ResolutionX = 640;
                m_ResolutionY = 360;
                break;
            case 1:
                m_ResolutionX = 1280;
                m_ResolutionY = 720;
                break;
            case 2:
                m_ResolutionX = 1920;
                m_ResolutionY = 1080;
                break;
            case 3:
                m_ResolutionX = 2560;
                m_ResolutionY = 1440;
                break;
            case 4:
                m_ResolutionX = 3840;
                m_ResolutionY = 2160;
                break;
            default:
                m_ResolutionX = 1920;
                m_ResolutionY = 1080;
                break;
        }
        if (applyValues) ApplyAll();
    }

    /// <summary>
    /// Applies all resolution settings.
    /// </summary>
    private static void ApplyAll() {
        Screen.SetResolution(m_ResolutionX, m_ResolutionY, m_DisplayMode);
        Debug.Log("[GraphicSettings]: Values were set to: Resolution: [" + m_ResolutionX + "x" + m_ResolutionY + "] DisplayMode: [" + m_DisplayMode + "]");
    }

}
