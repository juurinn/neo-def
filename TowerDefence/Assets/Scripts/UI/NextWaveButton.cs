using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class NextWaveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private TMP_Text text;
    private bool isActive;

    private void Awake() {
        text = ReferencesUI.instance.nextWaveTooltip.GetComponentInChildren<TMP_Text>();
    }

    private void Update() {
        ReferencesUI.instance.bonus.text = "<color=green>"+GameManager.instance.currentWaveReward.ToString()+"%</color>";
        if (!isActive) return;

        SetText(GameManager.instance.currentWaveReward);
    }


    private void SetText(int percent) {
        text.text = "Press to start the next wave immediately. If you survive the wave by killing all of the enemies before they reach the end, your reward is increased by <color=#00ff00ff>" + percent + "%</color>";
    }


    public void OnPointerEnter(PointerEventData eventData) {
        isActive = true;
        ReferencesUI.instance.nextWaveTooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        isActive = false;
        ReferencesUI.instance.nextWaveTooltip.SetActive(false);
    }
}
