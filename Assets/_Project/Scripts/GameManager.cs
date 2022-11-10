using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public GameObject playerPrefab;
    public string saveName;
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

        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();

        if (saveData.levelNumber == -1)
        {
            SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTestField()
    {
        saveName = "test";
        saveData = SaveSystem.Load(saveName);
        LoadSkillsData();
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void LoadSkillsData()
    {
        skills[0].GetComponent<SkillHeal>().sumOfHealingDone = saveData.skillHealProgress;
        if (saveData.skillHealProgress <= skills[0].GetComponent<SkillHeal>().healingForLevelOne)
        {
            skills[0].GetComponent<SkillController>().mastering = saveData.skillHealProgress / skills[0].GetComponent<SkillHeal>().healingForLevelOne;
        }
        else
        {
            skills[0].GetComponent<SkillController>().mastering = 1 + saveData.skillHealProgress / skills[0].GetComponent<SkillHeal>().healingForLevelTwo;
        }
        
    }
}
