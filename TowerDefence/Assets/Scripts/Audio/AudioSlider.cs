using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages saving and loading audio volume and handles slider values.
/// </summary>
public class AudioSlider : MonoBehaviour {

    [Header("References")]
    [Tooltip("Target audio slider goes here.")]
    public Slider m_Slider;
    [Tooltip("Which audio mixer group does this slider represent.")]
    public AudioMixerGroup m_AudioMixerGroup;
    [Tooltip("Volume percentage text goes here.")]
    public TMP_Text m_VolumePercentageText;
    [Tooltip("Mixer parameter to affect, this is created in audio mixer.")]
    public string m_MixerParameter;

    private float m_LastSliderValue = 0f;
    private bool m_Initialized = false;

    private void OnEnable() {
        if (!m_Initialized) return;
        SetInitialVolume(PlayerPrefs.GetFloat("MasterVolume", Config.DEFAULT_AUDIO_VOLUME));
    }

    private void Start() {
        SetInitialVolume(PlayerPrefs.GetFloat("MasterVolume", Config.DEFAULT_AUDIO_VOLUME));
        m_Initialized = true;
    }

    /// <summary>
    /// Sets initial volume settings.
    /// </summary>
    /// <param name="initialVolume"></param>
    private void SetInitialVolume(float initialVolume) {
        UpdateMixer(initialVolume);
        UpdateSlider(initialVolume);
    }

    /// <summary>
    /// <para> Updates slider % and heard volume. </para>
    /// <para> Call this from OnValueChanged, this only adjusts the seen and heard value and does not save them. </para>
    /// </summary>
    /// <param name="sliderValue"></param>
    public void SetVolume(float sliderValue) {
        m_LastSliderValue = sliderValue;
        UpdateMixer(sliderValue);
        m_VolumePercentageText.text = "Volume: " + Mathf.Round(m_Slider.value * 100f).ToString() + "%";
    }

    /// <summary>
    /// <para> Saves slider value to player prefs. </para>
    /// <para> Call this from PointerClick and EndDrag. [Has a slight issue of saving twice in certain cases] </para>
    /// </summary>
    public void SaveSliderValue() {
        PlayerPrefs.SetFloat("MasterVolume", m_LastSliderValue);
    }

    /// <summary>
    /// Sets volume parameter in audio mixer.
    /// </summary>
    /// <param name="newVolume"> Slider value. </param>
    private void UpdateMixer(float newVolume) {
        float m_SliderValueLogarithm = Mathf.Log10(newVolume) * 20;
        m_AudioMixerGroup.audioMixer.SetFloat(m_MixerParameter, m_SliderValueLogarithm);
    }

    /// <summary>
    /// Updates slider values.
    /// </summary>
    /// <param name="newVolume"> Slider value. </param>
    private void UpdateSlider(float newVolume) {
        m_Slider.value = newVolume;
        m_VolumePercentageText.text = "Volume: " + Mathf.Round(m_Slider.value * 100f).ToString() + "%";
    }

}
