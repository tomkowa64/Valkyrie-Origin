using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Do not touch")]
    public SaveData saveData;
    public int[] unlockedSkills;
    private static GameManager instance;
    private string newSaveName;

    [Header("To fill")]
    public GameObject playerPrefab;
    public GameObject communicate;
    public GameObject[] skills;

    [Header("If you want to use sandbox set it to name of your save")]
    public string saveName;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SaveNameInput(string value)
    {
        newSaveName = value;
        communicate.SetActive(false);
    }

    public void CreateNewSave()
    {
        if (newSaveName != null && newSaveName != "")
        {
            if (SaveSystem.GetAllSaves().Count >= 5)
            {
                communicate.GetComponent<TextMeshProUGUI>().text = "You have reached max saves count";
                communicate.SetActive(true);
                Debug.LogError("You have reached max saves count");
            }
            else
            {
                if (SaveSystem.SaveExists(newSaveName))
                {
                    communicate.GetComponent<TextMeshProUGUI>().text = "Save with that name already exists";
                    communicate.SetActive(true);
                    Debug.LogError("Save with that name already exists");
                }
                else
                {
                    SaveSystem.NewSave(newSaveName);
                    saveName = newSaveName;
                    LoadGame();
                }
            }
        }
        else
        {
            communicate.GetComponent<TextMeshProUGUI>().text = "Save name cannot be empty";
            communicate.SetActive(true);
            Debug.LogError("Cannot create save with no name");
        }
    }

    public void LoadGame()
    {
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();

        if (saveData.levelNumber >= 0)
        {
            SceneManager.LoadScene("Level" + saveData.levelNumber, LoadSceneMode.Single);
        }
        else if (saveData.levelNumber == -1)
        {
            SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
        }
    }

    public void LoadTestField()
    {
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
    }

    private void LoadSkillsData()
    {
        unlockedSkills = saveData.unlockedSkills;

        #region Heal data
        skills[0].GetComponent<SkillHeal>().sumOfHealingDone = saveData.skillHealProgress;
        if (saveData.skillHealProgress <= skills[0].GetComponent<SkillHeal>().healingForLevelOne)
        {
            skills[0].GetComponent<SkillController>().mastering = saveData.skillHealProgress / skills[0].GetComponent<SkillHeal>().healingForLevelOne;
        }
        else
        {
            skills[0].GetComponent<SkillController>().mastering = 1 + saveData.skillHealProgress / skills[0].GetComponent<SkillHeal>().healingForLevelTwo;
        }
        #endregion

        #region HP Regen data
        skills[1].GetComponent<SkillHPRegen>().sumOfHealthRegen = saveData.skillHealthRegenProgeess;
        skills[1].GetComponent<SkillController>().mastering = saveData.skillHealthRegenProgeess / skills[1].GetComponent<SkillHPRegen>().regenForLevelOne;
        #endregion

        #region Charge data
        skills[2].GetComponent<SkillCharge>().sumOfEnemiesHit = saveData.skillChargeProgress;
        skills[2].GetComponent<SkillController>().mastering = saveData.skillChargeProgress / skills[2].GetComponent<SkillCharge>().hitsForLevelOne;
        #endregion
    }
}
