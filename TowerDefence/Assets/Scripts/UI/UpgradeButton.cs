using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Handles tower upgrade button.
/// </summary>
public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private GameObject _Button;
    private TMP_Text _costTxt;
    private TurretBlueprint baseBp;
    private TurretBlueprint currentBp;
    private TurretBlueprint upgradeBp;

    public string BtnTxt;
    private bool isLeft;
    private bool isHovered = false; // Is button currently hovered

    private void Awake() {
        _Button = GetComponentInChildren<Button>().gameObject;
        _costTxt = _Button.GetComponentInChildren<TMP_Text>();
        _Button.SetActive(false);
    }


    /// <summary>
    /// Enable button.
    /// </summary>
    public void Enable(Turret _Turret, int inc = 1) {
        baseBp = TurretConfig.Get(_Turret.towerIndex);
        currentBp = _Turret.blueprint;
        upgradeBp = TurretConfig.Get(_Turret.towerIndex, _Turret.upgradeTier + inc);

        isLeft = (_Turret.upgradeTier + inc) % 2 == 1;

        _Button.SetActive(true);
        _costTxt.text = BtnTxt + " " + upgradeBp.cost + "$";

        // Make sure that UI is shown
        if (isHovered) DisplayUpgradeUI();
    }


    /// <summary>
    /// Disable Button. 
    /// </summary>
    public void Disable() {
        References.coneOfVisionUpgrade.gameObject.SetActive(false);
        if (_Button != null) _Button.SetActive(false);
        isHovered = false;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        isHovered = true;
        DisplayUpgradeUI();
    }


    public void OnPointerExit(PointerEventData eventData) {
        References.coneOfVisionUpgrade.gameObject.SetActive(false);
        ReferencesUI.instance.towerInfo.SetActive(false);
        isHovered = false;
    }


    /// <summary>
    /// Displays upgrade UI.
    /// </summary>
    private void DisplayUpgradeUI() {
        if (upgradeBp.range != 0)
            TowerManager.instance.ActivateRangeIndicator(upgradeBp.range);

        UIManager.instance.SetTowerInfoForUpgrade(
            GetRichText("Dmg: ", upgradeBp.dmg, currentBp.dmg),
            GetRichText("Range: ", upgradeBp.range, currentBp.range),
            GetRichText("Fire Rate: ", upgradeBp.fireRate, currentBp.fireRate, "/s"),
            upgradeBp.name != "" ? upgradeBp.name : baseBp.name,
            upgradeBp.description + "\n" + TurretConfig.TowerStatDesc(upgradeBp, baseBp.type, isLeft)
        );
    }


    /// <summary>
    /// Get upgrade UI stat value as richtext. Color depends on change in stat.
    /// </summary>
    private string GetRichText(string prefix, float newValue, float oldValue, string suffix = "") {
        if (newValue == 0 || newValue == oldValue)
            return prefix + "<color=white>" + oldValue + "</color>" + suffix;
        else if (newValue < oldValue)
            return prefix + "<color=red>" + newValue + "</color>" + suffix;
        else
            return prefix + "<color=green>" + newValue + "</color>"+ suffix;
    }
}
