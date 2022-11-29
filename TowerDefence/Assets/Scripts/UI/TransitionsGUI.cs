using UnityEngine;

/// <summary>
/// Handles playing animations. (For button events)
/// </summary>
public class TransitionsGUI : MonoBehaviour {

    public Animator preGameAnimator;

    public void PlayPregameExit() {
        preGameAnimator.SetBool("isGameStarted", true);
    }

}
