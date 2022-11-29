using System;
using UnityEngine;

/// <summary>
/// Manages the in game time.
/// </summary>
public class TimeManager : MonoBehaviour {

    static public TimeManager instance;

    [Header("Speed")]
    [Tooltip("How fast is the normal time scale.")]
    public float normalTimeScale = 1f;
    [Tooltip("How fast is the sped up time scale.")]
    public float fastTimeScale = 5f;

    /// <summary>
    /// The current, real time, time scale.
    /// </summary>
    public float CurrentTimeScale { get; private set; } = 1f;

    private float m_InitialFixedDeltaTime = 0f;
    private float m_InitialMaximumDeltaTime = 0f;

    /// <summary>
    /// Is current timescale fast.
    /// </summary>
    private bool isSpedUp = false;

    private Action<float> onTimeScaleToggle;

    private void OnDestroy() {
        ApplyTimeScale(normalTimeScale);
    }


    private void Awake() {
        if (instance == null) {
            instance = this;
            Initialization();
        } else {
            Debug.LogError("[TimeManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }

        AddTimeScaleToggleHandler(ApplyTimeScale);
    }


    private void Start() {
        ReferencesUI.instance.gameSpeedToggleButtonText.text = "<color=#A3A4A2>[Tab]</color> Speed x " + CurrentTimeScale;
    }


    public void AddTimeScaleToggleHandler(Action<float> action) => onTimeScaleToggle += action;

    /// <summary>
    /// Initializes time manager.
    /// </summary>
    private void Initialization() {
        m_InitialFixedDeltaTime = Time.fixedDeltaTime;
        m_InitialMaximumDeltaTime = Time.maximumDeltaTime;
        ApplyTimeScale(normalTimeScale);
    }


    /// <summary>
    /// Toggle timescale between fast and normal, update UI accordingly.
    /// </summary>
    public void ToggleTimeScale() {
        isSpedUp = !isSpedUp;
        onTimeScaleToggle.Invoke(isSpedUp ? fastTimeScale : normalTimeScale);

        ReferencesUI.instance.gameSpeedToggleButtonText.text = "<color=#A3A4A2>[Tab]</color> Speed x " + CurrentTimeScale;
    }


    /// <summary>
    /// Modifies the timescale and time attributes to match the new timescale.
    /// </summary>
    /// <param name="newValue"> New time scale to apply.  </param>
    public void ApplyTimeScale(float newValue) {
        Time.timeScale = newValue;

        if (newValue != 0) {
            Time.fixedDeltaTime = m_InitialFixedDeltaTime * newValue;
        }
        Time.maximumDeltaTime = m_InitialMaximumDeltaTime * newValue;

        CurrentTimeScale = Time.timeScale;
    }


    /// <summary>
    /// Resets time scale back to normal.
    /// </summary>
    public void ResetTimeScale() {
        ApplyTimeScale(normalTimeScale);
    }

}
