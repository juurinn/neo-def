using UnityEngine;

public class Shop : MonoBehaviour {

    [Header("Turrets")]
    public TurretBuildBlueprint standardTurret;
    public TurretBuildBlueprint laserTurret;
    public TurretBuildBlueprint slowTurret;
    public TurretBuildBlueprint bleedTurret;
    public TurretBuildBlueprint splashTurret;
    public TurretBuildBlueprint sniperTurret;

    public static Shop instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[Shop]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Select a turret by index
    /// </summary>
    /// <param name="index"> Turret index </param>
    public void SelectTurret(int index) {
        switch (index) {
            case 0:
                BuildManager.instance.SelectTurretToBuild(standardTurret);
                break;
            case 1:
                BuildManager.instance.SelectTurretToBuild(laserTurret);
                break;
            case 2:
                BuildManager.instance.SelectTurretToBuild(slowTurret);
                break;
            case 3:
                BuildManager.instance.SelectTurretToBuild(bleedTurret);
                break;
            case 4:
                BuildManager.instance.SelectTurretToBuild(splashTurret);
                break;
            case 5:
                BuildManager.instance.SelectTurretToBuild(sniperTurret);
                break;
            default:
                Debug.LogWarning("[Shop]: Unknown turret index while slecting turret!");
                break;
        }
    }

}
