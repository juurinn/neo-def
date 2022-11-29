using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Blinks the text element when you put it in the gameobject
/// </summary>
public class TextBlinker : MonoBehaviour {
    private TMP_Text texttoblink;
    [Space(10f)]
    [SerializeField, Range(0.1f, 1f)] private float blinkSpeed = 0.5f;
    [Space(15f)]
    [SerializeField] private Color blinkColor1 = Color.red;
    [SerializeField] private Color blinkColor2 = Color.white;

    private void Awake() {
        texttoblink = GetComponent<TMP_Text>();
    }

    private void OnEnable() {
        StartCoroutine(TextBlink());
    }

    private void OnDisable() {
        StopCoroutine(TextBlink());
    }

    private IEnumerator TextBlink() {
        while (true) {
            texttoblink.color = blinkColor1;
            yield return new WaitForSeconds(blinkSpeed);
            texttoblink.color = blinkColor2;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }



}
