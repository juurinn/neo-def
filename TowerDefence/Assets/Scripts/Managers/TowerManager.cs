using System;
using UnityEngine;

/// <summary>
/// Handle Selected tower overlay.
/// </summary>
public class TowerManager : MonoBehaviour {

    public static TowerManager instance;

    public GameObject _Target;
    public GameObject _TargetOuter;

    public Material visionIncrease;
    public Material visionDecrease;

    private ConeOfVision2D visionCone { get => References.coneOfVision; }
    private MeshRenderer visionUpgradeRenderer;

    private Turret _Tower;

    /// <summary>
    /// Do we have tower selected currently.
    /// </summary>
    public bool IsTowerSelected { get => _Tower != null; }


    private void Awake() {
        if (instance == null) instance = this;
        else {
            Debug.LogError("[TowerManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
            return;
        }
    }


    private void Start() {
        visionUpgradeRenderer = References.coneOfVisionUpgrade.GetComponent<MeshRenderer>();
    }


    private void Update() {
        if (!IsTowerSelected) return;
        //ReferencesUI.instance.selectedTowerTotalDmg.text = "Total dmg: " + (int)_Tower.totalDmg;
    }


    #region Public Functions


    /// <summary>
    /// Try selecting tower.
    /// </summary>
    public void TrySelectTower() {
        // Get turret, return if turret not found
        if (!GetTower()) {
            DeSelectTower();
            return;
        }

        UpdateUI();
        ActivateVisionMesh(visionCone, _Tower.blueprint.range);
        if (_Target != null) {
            _Target.SetActive(true);
            _Target.transform.position = _Tower.transform.position;
        }
    }


    /// <summary>
    /// DeSelects selected tower.
    /// </summary>    
    public void DeSelectTower() {
        ReferencesUI.instance.selectedTowerInfo.SetActive(false);
        References.coneOfVisionUpgrade.gameObject.SetActive(false);
        _Tower = null;
        DeactivateVisionMesh();

        _Target?.SetActive(false);
        _TargetOuter.transform.localScale = new Vector3(1.3f, 1.3f, 1);
    }


    /// <summary>
    /// Sell the selected tower.
    /// </summary>
    public void SellTower() {
        if (!IsTowerSelected) return;

        // Tiledata position = transform position rounded down
        int towerPosX = (int)_Tower.gameObject.transform.position.x;
        int towerPosY = (int)_Tower.gameObject.transform.position.y;

        // Reward player
        Currency.Use(Mathf.RoundToInt(_Tower.sellPrice * Config.SELL_MULTIPLIER));
        // Destroy the tower
        Destroy(_Tower.gameObject);
        GridManager.instance.SetTileToGround(towerPosX, towerPosY);

        DeSelectTower();
    }


    /// <summary>
    /// Change targeting mode of selected tower.
    /// </summary>
    public void ChangeTargetingMode(int i) {
        if (!IsTowerSelected) return;

        // Get new targeting mode and set it to turret
        int newIndex = GetTMIndex(i);
        _Tower.m_TargetingMode = (TargetingMode)newIndex;

        UpdateUI();
    }


    /// <summary>
    /// Upgrade selected tower, refresh UI and VisionMesh.
    /// </summary>
    /// <param name="inc">Increase to the current upgrade tier. 1: next/choice1 | 2: choice2</param>
    public void UpgradeTower(int inc, bool choiceBtn = false) {
        if (!IsTowerSelected) return;

        // Dont allow choice buttons when normal upgrade and viceversa
        if (choiceBtn && _Tower.upgradeTier != 0 || !choiceBtn && _Tower.upgradeTier == 0) return;

        if (_Tower.upgradeTier > 4) return;

        // Get upgrade 
        TurretBlueprint upgrade = TurretConfig.Get(_Tower.towerIndex, _Tower.upgradeTier + inc);

        // If can afford to afford to upgrade
        if (Currency.Use(-upgrade.cost)) {
            _Tower.Upgrade(upgrade, _Tower.upgradeTier + inc);
            UpdateUI();
            UpdateVisionMesh();
        } else {
            UserNotification.instance.RaiseError(ErrorMsgCode.cantAffordUpgrade);
        }
    }


    /// <summary>
    /// Activate conevision for displaying range after upgrade.
    /// </summary>
    public void ActivateRangeIndicator(float range) {
        visionUpgradeRenderer.material = range > _Tower.blueprint.range ? visionIncrease : visionDecrease;
        ActivateVisionMesh(References.coneOfVisionUpgrade, range);
    }


    #endregion
    #region Private Functions


    /// <summary>
    /// Activates turret vision mesh.
    /// </summary>
    private void ActivateVisionMesh(ConeOfVision2D cone, float range) {
        cone.gameObject.SetActive(true);
        cone.VisionRadius = range;
        cone.gameObject.transform.position = _Tower.transform.position;
        cone.UpdateMesh();
    }


    /// <summary>
    /// Deactivates turret vision mesh.
    /// </summary>
    private void DeactivateVisionMesh() {
        visionCone.gameObject.SetActive(false); // Deactivate vision
    }


    /// <summary>
    /// Use this when turret vision is changed through turret upgrade!
    /// </summary>
    private void UpdateVisionMesh() {
        visionCone.VisionRadius = _Tower.blueprint.range;
        visionCone.UpdateMesh();
    }


    /// <summary>
    /// Update selected tower overlay.
    /// </summary>
    private void UpdateUI() {
        TurretBlueprint t = _Tower.blueprint;
        UIManager.instance.SetSelectedTower(
            t.name,
            t.dmg,
            t.range,
            t.fireRate,
            Enum.GetName(typeof(TargetingMode),
            _Tower.m_TargetingMode),
            Mathf.RoundToInt(_Tower.sellPrice * Config.SELL_MULTIPLIER),
            TurretConfig.TowerStatDesc(_Tower.blueprint, _Tower.blueprint.type, (_Tower.upgradeTier) % 2 == 1)
        );

        // Set UpgradeButtons if upgrades left
        if (_Tower.upgradeTier < 5)
            UIManager.instance.SetTowerUpgradeButtons(_Tower, _Tower.upgradeTier == 0);
        else
            UIManager.instance.DisableTowerUpgradeButtons();
    }


    /// <summary>
    /// Selects the obj if one clicked.
    /// </summary>
    private bool GetTower() {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, short.MaxValue);
        if (hit) hit.collider.TryGetComponent<Turret>(out _Tower);
        else _Tower = null;

        return _Tower != null;
    }


    /// <param name="d">1 or -1</param>
    /// <param name="curr">current Targeting mode</param>
    /// <returns>Updated enum index</returns>
    private int GetTMIndex(int d) {
        int curr = (int)_Tower.m_TargetingMode;
        int max = Enum.GetNames(typeof(TargetingMode)).Length - 1;

        if (curr == max && d == 1) return 0;
        else if (curr == 0 && d == -1) return max;
        else return curr + d;
    }

    #endregion
}
