using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// References to different UI elements for easier access
/// </summary>
public class ReferencesUI : MonoBehaviour {
    public static ReferencesUI instance;

    public GameObject pregameGUI;

    [Space(10), Header("Gamestate"), Space(20)]
    public TMP_Text waveTimer;
    public TMP_Text bonus;

    [Space(10), Header("InGame buttons"), Space(20)]
    public TMP_Text gameSpeedToggleButtonText;
    public Button nextWaveBtnContainer;

    [Space(10), Header("TowerInfo overlaly"), Space(20)]
    public GameObject towerInfo;
    public TMP_Text towerInfoName;
    public TMP_Text towerInfoDamage;
    public TMP_Text towerInfoFireRate;
    public TMP_Text towerInfoRange;
    public TMP_Text towerInfoDescription;

    [Space(10), Header("SelectedTowerInfo overlaly"), Space(20)]
    public GameObject selectedTowerInfo;
    public TMP_Text selectedTowerInfoName;
    public TMP_Text selectedTowerInfoDamage;
    public TMP_Text selectedTowerInfoFirerate;
    public TMP_Text selectedTowerInfoRange;
    public TMP_Text selectedTowerInfoTargetingMode;
    public TMP_Text selectedTowerDescription;
    public TMP_Text selectedTowerSellText;
    public TMP_Text selectedTowerTotalDmg;
    public UpgradeButton towerUpgradeButton1;
    public UpgradeButton towerUpgradeButton2;
    public UpgradeButton towerUpgradeButton3;

    [Space(10), Header("UserNotification"), Space(20)]
    public TMP_Text notificationText;
    public Animator notificationAnimator;
    public TMP_Text errorNotificationText;
    public Animator errorNotificationAnimator;
    [Space(20)]
    public GameObject nextWaveTooltip;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[ReferencesUI]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }

}
