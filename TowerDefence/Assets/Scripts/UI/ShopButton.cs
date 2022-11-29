using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles showing and updating tower information
/// </summary>
public class ShopButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public int m_TowerIndex;

    public void OnPointerEnter(PointerEventData eventData) {
        if (!BuildManager.instance.CanBuild) {
            UIManager.instance.SetTowerInfo(TurretConfig.Get(m_TowerIndex));
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!BuildManager.instance.CanBuild) {
            ReferencesUI.instance.towerInfo.SetActive(false);
        }
    }
}
