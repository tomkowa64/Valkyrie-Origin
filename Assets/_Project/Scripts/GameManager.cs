using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public GameObject playerPrefab;
    public string saveName;
    private string newSaveName;
    public SaveData saveData;
    public GameObject[] skills;

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

    /*
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();

        if (saveData.levelNumber == -1)
        {
            SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
        }
     */

    public void SaveNameInput(string value)
    {
        newSaveName = value;
    }

    public void CreateNewSave()
    {
        if (newSaveName != null && newSaveName != "")
        {
            if (SaveSystem.SaveExists(newSaveName))
            {
                Debug.LogError("Save with that name already exists");
            }
            else
            {
                SaveSystem.NewSave(newSaveName);
                saveName = newSaveName;
                LoadGame();
            }
        }
        else
        {
            Debug.LogError("Cannot create save with no name");
        }
    }

    public void LoadGame()
    {
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();
        SceneManager.LoadScene("Level" + saveData.levelNumber, LoadSceneMode.Single);
    }

    public void LoadTestField()
    {
        saveName = "test";
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
    }

    private void LoadSkillsData()
    {
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
