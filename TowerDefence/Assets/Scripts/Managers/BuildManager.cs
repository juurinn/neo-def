using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This manages turret building on the grid
/// </summary>
public class BuildManager : MonoBehaviour {

    public GameObject[] cursorIcons;

    private ConeOfVision2D visionCone { get => References.coneOfVision; }
    private Vector3Int lastGridPos;
    private TurretBuildBlueprint turretToBuild;

    public bool CanBuild { get => turretToBuild != null; }

    private GameObject m_CurrentActiveCursor;

    public static BuildManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[BuildManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
            return;
        }
    }


    private void LateUpdate() {
        // Update cursor icon when building
        if (CanBuild) {
            // Get hovered grid position
            Vector3Int gridPos = GridManager.instance.GetMouseGridPoint();
            Vector3 worldPos = GridManager.instance.GridToWorld(gridPos);
            m_CurrentActiveCursor.transform.position = worldPos;

            if (gridPos == lastGridPos) return;

            visionCone.gameObject.transform.position = worldPos;
            visionCone.UpdateMesh();

            lastGridPos = gridPos;
        }
    }


    /// <summary>
    /// Selects/unselects tower
    /// </summary>
    /// <param name="turret">  </param>
    public void SelectTurretToBuild(TurretBuildBlueprint turret) {
        // Close tower interact interface if open
        TowerManager.instance.DeSelectTower();

        // If selecting tower that is already selected, deselect
        if (turretToBuild == turret) {
            DeselectTurret();
        } else {
            turretToBuild = turret;

            UIManager.instance.SetTowerInfo(TurretConfig.Get(turret.m_TurretIndex));

            // Activate and update ConeVision with turret blueprint
            visionCone.Config(TurretConfig.Get(turret.m_TurretIndex));
            visionCone.UpdateMesh();

            ActivateCursor(turret.m_TurretIndex);
        }
    }

    private void ActivateCursor(int index) {
        m_CurrentActiveCursor?.SetActive(false);
        m_CurrentActiveCursor = cursorIcons[index];
        m_CurrentActiveCursor.SetActive(true);
    }

    /// <summary>
    /// Try placing selected turret.
    /// </summary>
    public void TryPlaceTurret() {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Get cost of tower, check if can afford
        int towerCost = TurretConfig.Get(turretToBuild.m_TurretIndex).cost;
        if (!Currency.CanAfford(towerCost)) {
            UserNotification.instance.RaiseError(ErrorMsgCode.cantAffordTower);
            return;
        }

        // Try placing turret, use currency if successful
        if (GridManager.instance.PlaceTower(turretToBuild.m_TurretPrefab, turretToBuild.m_TurretIndex)) {
            Currency.Use(-towerCost);
        }
    }


    /// <summary>
    /// Deselects turret that is being placed
    /// </summary>
    public void DeselectTurret() {
        turretToBuild = null;
        visionCone.gameObject.SetActive(false);
        ReferencesUI.instance.towerInfo.SetActive(false);
        m_CurrentActiveCursor?.SetActive(false);
        m_CurrentActiveCursor = null;
    }

}
