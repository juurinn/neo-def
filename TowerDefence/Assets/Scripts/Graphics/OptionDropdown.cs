using UnityEngine;
using TMPro;

public class OptionDropdown : MonoBehaviour {

    public enum DropdownType {
        None,
        DisplayMode,
        Resolution
    }

    public TMP_Dropdown m_Dropdown;
    public DropdownType m_DropdownType;

    private bool m_Initialized = false;

    private void OnEnable() {
        if (!m_Initialized) return;
        SetInitialValue();
    }

    private void Start() {
        SetInitialValue();
        m_Initialized = true;
    }

    /// <summary>
    /// Sets dropdown values to loaded values. [Does not update settings]
    /// </summary>
    private void SetInitialValue() {
        switch (m_DropdownType) {
            case DropdownType.None:
                Debug.LogError("[OptionDropdown]: You are trying to set dropdown type None!");
                break;
            case DropdownType.DisplayMode:
                m_Dropdown.value = LoadGraphicsSettings.DisplayMode;
                break;
            case DropdownType.Resolution:
                m_Dropdown.value = LoadGraphicsSettings.Resolution;
                break;
            default:
                Debug.LogError("[OptionDropdown]: Unkown dropdown type! Could not set correct dropdown value!");
                break;
        }
    }

    /// <summary>
    /// Saves selected dropdown value and applys it.
    /// </summary>
    /// <remarks>
    /// <para> Call this from OnValueChanged. </para>
    /// </remarks>
    public void SaveValue() {
        if (!m_Initialized) return;
        switch (m_DropdownType) {
            case DropdownType.None:
                Debug.LogError("[OptionDropdown]: You are trying to save dropdown type None!");
                break;
            case DropdownType.DisplayMode:
                PlayerPrefs.SetInt("DisplayMode", m_Dropdown.value);
                LoadGraphicsSettings.DisplayMode = m_Dropdown.value;
                LoadGraphicsSettings.SetDisplayMode(true);
                break;
            case DropdownType.Resolution:
                PlayerPrefs.SetInt("Resolution", m_Dropdown.value);
                LoadGraphicsSettings.Resolution = m_Dropdown.value;
                LoadGraphicsSettings.SetResolution(true);
                break;
            default:
                Debug.LogError("[OptionDropdown]: Unkown dropdown type! Could not save correct dropdown value!");
                break;
        }
    }

}
