using UnityEngine;

/// <summary>
/// For testing in editor.
/// </summary>
public class CapFrameRate : MonoBehaviour {
    #if UNITY_EDITOR

    public int frameRate = 60;

    private void Start() {
        Application.targetFrameRate = frameRate;
    }

    #endif
}
