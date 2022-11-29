using TMPro;
using System;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private float timeBetweenWaves;

    [Header("UI Elements")]
    public TMP_Text livesTxt;
    public TMP_Text wavesTxt;
    public TMP_Text waveTimer;
    public TMP_Text currencyText;
    public GameObject GameOverCanvas;
    public TMP_Text[] TowerCost;

    [Header("Random seed")]
    public TMP_InputField SeedInput;
    public TMP_Text SeedWM;


    [Header("GameOver")]
    public TMP_Text wavesSurvivedText;
    public TMP_InputField GameOverSeed;

    public static UIManager instance;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Debug.LogError("[UIManager]: Trying to instance but another instance already exists! [" + gameObject + "]");
            Destroy(gameObject);
        }
    }


    private void Start() {
        timeBetweenWaves = Config.TIME_BETWEEN_WAVES;
        SeedInput.text = Seed.instance.seed.ToString();
        SeedWM.text = Seed.instance.seed.ToString();
        GameOverSeed.text = Seed.instance.seed.ToString();
    }


    /// <summary>
    /// Applies custom seed
    /// </summary>
    public void SetCustomSeed() {
        int seed = 0;

        if (SeedInput.text == "-") {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Seed.instance.customSeed = seed;
            GameManager.instance.RestartGame();
            return;
        }

        long seedLong = long.Parse(SeedInput.text);

        // Value could be also set to 0 to randomize new seed if it is invalid
        if (seedLong > int.MaxValue) {
            seed = int.MaxValue;
        } else if (seedLong < int.MinValue) {
            seed = int.MinValue;
        } else {
            seed = Mathf.Clamp((int)seedLong, Int32.MinValue, Int32.MaxValue);
        }

        Seed.instance.customSeed = seed;
        GameManager.instance.RestartGame();
    }

    public void SetTowerCosts() {
        for (int i = 0; i < TowerCost.Length; i++) {
            TowerCost[i].text = TurretConfig.Get(i).cost.ToString();
        }
    }


    public Vector3 UpdateMousePos() {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        Vector3 playerPos = Camera.main.ScreenToWorldPoint(mousePos);
        return playerPos;
    }


    public void UpdateTimer(float elapsedTime) {
        waveTimer.text = TimeSpan.FromSeconds(timeBetweenWaves - elapsedTime).ToString("ss") + "s";
        if (timeBetweenWaves - elapsedTime > 10)
            waveTimer.color = new Color(1, 1, 1, 1); //white
        else if (timeBetweenWaves - elapsedTime <= 10 && timeBetweenWaves - elapsedTime > 5)
            waveTimer.color = new Color(1, 1, 0, 1); // yellow
        else if (timeBetweenWaves - elapsedTime <= 5)
            waveTimer.color = new Color(1, 0, 0, 1); // red 
    }

    public void UpdateTextElement(string element, string value) {
        TMP_Text textToBeUpdated = null;
        switch (element) {
            case "currency":
                textToBeUpdated = currencyText;
                break;
            case "lives":
                textToBeUpdated = livesTxt;
                break;
            case "waves":
                textToBeUpdated = wavesTxt;
                break;
            default:
                Debug.LogError("[UIManager]: Invalid element [" + element + "]");
                break;
        }

        textToBeUpdated.text = value;
    }


    public void MoneyHack() {
        if (GameManager.instance.devMode) UseCurrency(10000);
    }


    /// <summary>
    /// <para>Use currency, add or remove. Updates the currency on GUI.</para>
    /// <para>Invokes: <see cref="Currency.Use(int)"/></para>
    /// </summary>
    /// <param name="m_Amount"> Amount of currency to be changed. Positive values for adding. | Negative values for removing. </param>
    public void UseCurrency(int m_Amount) =>
        Currency.Use(m_Amount);



    public void UpgradeTower(int buttonId) {
        TowerManager.instance.UpgradeTower(buttonId > 2 ? 2 : buttonId, buttonId < 3);
    }


    public void NextWave() => AI.instance.StartWave(GameManager.instance.currentWaveReward);


    /// <summary>
    /// Enable/Disable NextWaveButton depending on if amount of waves running currently is at max.
    /// </summary>
    public void SetNextWaveButton(int amountOfWavesRunningCurrently) {
        ReferencesUI.instance.nextWaveBtnContainer.interactable = amountOfWavesRunningCurrently < Config.MAX_WAVES_RUNNING;
    }


    public void SetTowerUpgradeButtons(Turret _Turret, bool isChoiceUpgrade) {
        if (isChoiceUpgrade) SetTowerUpgradeButtonsChoice(_Turret);
        else SetTowerUpgradeButtonsBasic(_Turret);
    }


    /// <summary>
    /// Enable and display TowerInfo.
    /// </summary>
    /// <param name="_turret">Blueprint of tower to be displayed.</param>
    public void SetTowerInfo(TurretBlueprint _turret) {
        ReferencesUI.instance.towerInfo.SetActive(true);
        ReferencesUI.instance.towerInfoRange.text = "Range: " + _turret.range;
        ReferencesUI.instance.towerInfoDamage.text = "Dmg: " + _turret.dmg;
        ReferencesUI.instance.towerInfoFireRate.text = "Fire Rate: " + _turret.fireRate + "/s";
        ReferencesUI.instance.towerInfoName.text = _turret.name;
        ReferencesUI.instance.towerInfoDescription.text = _turret.description + "\n" + TurretConfig.TowerStatDesc(_turret, _turret.type, false);
    }


    /// <summary>
    /// Enable and display TowerInfo used as UpgradeInfo.
    /// </summary>
    public void SetTowerInfoForUpgrade(string damage, string range, string firerate, string name, string description) {
        ReferencesUI.instance.towerInfo.SetActive(true);
        ReferencesUI.instance.towerInfoName.text = name;
        ReferencesUI.instance.towerInfoRange.text = range;
        ReferencesUI.instance.towerInfoDamage.text = damage;
        ReferencesUI.instance.towerInfoFireRate.text = firerate;
        ReferencesUI.instance.towerInfoDescription.text = description;
    }


    public void SetSelectedTower(string name, float damage, float range, float firerate, string targetingmode, int sellPrice, string description) {
        ReferencesUI.instance.selectedTowerInfo.SetActive(true);
        ReferencesUI.instance.selectedTowerInfoName.text = name;
        ReferencesUI.instance.selectedTowerInfoRange.text = "Range: " + range;
        ReferencesUI.instance.selectedTowerInfoDamage.text = "Dmg: " + damage;
        ReferencesUI.instance.selectedTowerInfoFirerate.text = "Fire Rate: " + firerate + "/s";
        ReferencesUI.instance.selectedTowerInfoTargetingMode.text = targetingmode;
        ReferencesUI.instance.selectedTowerSellText.text = "<color=#A3A4A2>[S]</color> SELL " + sellPrice + "$";
        ReferencesUI.instance.selectedTowerDescription.text = description;
    }


    public void SetTowerUpgradeButtonsBasic(Turret _Turret) {
        ReferencesUI.instance.towerUpgradeButton2.Disable();
        ReferencesUI.instance.towerUpgradeButton3.Disable();
        ReferencesUI.instance.towerUpgradeButton1.Enable(_Turret, 2);
    }


    public void SetTowerUpgradeButtonsChoice(Turret _Turret) {
        ReferencesUI.instance.towerUpgradeButton1.Disable();
        ReferencesUI.instance.towerUpgradeButton2.Enable(_Turret);
        ReferencesUI.instance.towerUpgradeButton3.Enable(_Turret, 2);
    }


    public void DisableTowerUpgradeButtons() {
        ReferencesUI.instance.towerUpgradeButton1.Disable();
        ReferencesUI.instance.towerUpgradeButton2.Disable();
        ReferencesUI.instance.towerUpgradeButton3.Disable();
        ReferencesUI.instance.towerInfo.SetActive(false);
    }


    /// <summary>
    /// Handle GameOver.
    /// </summary>
    public void OnGameOver() {
        wavesSurvivedText.text = "You survived: " + GameManager.instance.WavesSurvived.ToString() + " Waves";
        GameOverCanvas.SetActive(true);
    }


    /// <summary>
    /// Handle GameSpeedToggle click. 
    /// </summary>
    public void OnGameSpeedToggleClick() {
        TimeManager.instance.ToggleTimeScale();
    }
}