using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class UIManager : MonoBehaviour
{
    public GameObject difficultyMenu;
    public GameObject gamePanel;
    public GameObject playBtn;
    public GameObject restartBtn;
    public GameObject resumeBtn;
    public GameObject languageMenu;
    public GameObject showSettingsScreenBtn;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private GameObject settingsMenu;

    public Text CounterText;
    public Text LostBallsText;
    public Text FailedShootsText;
    public Text MissedBallsText;
    public Text ShootingBallsText;
    public Text CollectedBallsText;

    public GameObject autoShootSwitch;
    public Toggle semiAutomaticToggle;
    public Toggle manualOrAutomaticToggle;

    private GameActionType lastAction;


    private LocalizedString counterLocalizedString;
    private LocalizedString lostBallsLocalizedString;
    private LocalizedString failedShootsLocalizedString;
    private LocalizedString missedBallsLocalizedString;
    private LocalizedString shootingBallsLocalizedString;
    private LocalizedString collectedBallsLocalizedString;

    private List<Locale> locales;

    void Start()
    {
        difficultyMenu.SetActive(false);
        restartBtn.SetActive(false);
        resumeBtn.SetActive(false);
        gamePanel.SetActive(false);

        locales = LocalizationSettings.AvailableLocales.Locales;
        dropdown.ClearOptions();

        var options = new List<string>();
        int currentIndex = 0;

        for(int i =0; i < locales.Count; i++)
        {
            options.Add(locales[i].LocaleName);

            if(locales[i] == LocalizationSettings.SelectedLocale)
                currentIndex = i;
        }

        dropdown.AddOptions(options);
        dropdown.value = currentIndex;
        dropdown.onValueChanged.AddListener(OnLanguageChaged);
        dropdown.gameObject.SetActive(false);
        settingsMenu.SetActive(false);

        counterLocalizedString = CounterText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
        lostBallsLocalizedString = LostBallsText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
        failedShootsLocalizedString = FailedShootsText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
        missedBallsLocalizedString = MissedBallsText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
        shootingBallsLocalizedString = ShootingBallsText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
        collectedBallsLocalizedString = CollectedBallsText.gameObject.GetComponent<LocalizeStringEvent>().StringReference;
    }

    private void OnLanguageChaged(int index)
    {
        LocalizationSettings.SelectedLocale = locales[index];
        settingsMenu.SetActive(false);
        dropdown.gameObject.SetActive(false);
        showSettingsScreenBtn.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameActionChanges(GameActionType actionType)
    {

        Debug.Log("Action Type: " + actionType);
        switch(actionType)
        {
            case GameActionType.ChooseDificulty:
                playBtn.SetActive(false);
                languageMenu.SetActive(false);
                difficultyMenu.SetActive(true);
                break;
            case GameActionType.Pause:
                resumeBtn.SetActive(true);
                restartBtn.SetActive(true);
                break;
            case GameActionType.Restart:
                resumeBtn.SetActive(false);
                restartBtn.SetActive(false);
                break;
            case GameActionType.Resume:
                resumeBtn.SetActive(false);
                restartBtn.SetActive(false);
                break;
            case GameActionType.GoToSettingsMenu:
                showSettingsScreenBtn.SetActive(false);
                settingsMenu.SetActive(true);
                dropdown.gameObject.SetActive(true);
                break;
        }

        lastAction = actionType;
    }
    public void OnGameDifficultyChange(GameDifficulty difficulty)
    {
        Debug.Log("difficulty: " + difficulty);
        difficultyMenu.SetActive(false);
        gamePanel.SetActive(true);
        // Start Game
    }

    public void SetBallsLost(int ballsLost)
    {
        // LostBallsText.text = $"Lost: {ballsLost}";

        ((IntVariable)lostBallsLocalizedString["value"]).Value = ballsLost;
        // localizedString.RefreshString();

    }

    public void SetScore(int points)
    {
        // CounterText.text = "Score : " + points;

        var handle = counterLocalizedString.GetLocalizedStringAsync();
        handle.Completed += op =>
        {
            IntVariable variable = counterLocalizedString["value"] as IntVariable;

            // if(variable == null)
            //     localizedString["score"] = new IntVariable{ Value = points };
            // else
            variable.Value = points;

            // counterLocalizedString.RefreshString();

        };
    }

    public void SetShootingBalls(int balls)
    {
        // ShootingBallsText.text = "Shooting Balls :" + balls;
        ((IntVariable)shootingBallsLocalizedString["value"]).Value = balls;
    }

    public void SetMissedShoots(int amount)
    {
        // MissedShootsText.text = "Missed Shoots : " + amount;
        ((IntVariable)failedShootsLocalizedString["value"]).Value = amount;
    }

    public void SetMissingBalls(int amount)
    {
        // MissedBallsText.text = "Missed Balls : " + amount;
        ((IntVariable)missedBallsLocalizedString["value"]).Value = amount;
    }

    public void SetCollectedBalls(int amount)
    {
        // CollectedBallsText.text = "Collected Balls : " + amount;
        ((IntVariable)collectedBallsLocalizedString["value"]).Value = amount;
    }

    public void OnShootingTypeChange(ShootingType st)
    {
        if(st == ShootingType.ManualOrAutomatic)
        {
            autoShootSwitch.SetActive(true);
            manualOrAutomaticToggle.isOn = true;
        }
        else
        {
            autoShootSwitch.SetActive(false);
            semiAutomaticToggle.isOn = true;
        }
    }
}
