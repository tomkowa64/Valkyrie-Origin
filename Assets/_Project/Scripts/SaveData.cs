using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public int levelNumber;
    public int checkpointNumber;
    public string locationName;

    #region Skills mastering 
    public float skillHealProgress;
    public float skillHealthRegenProgeess;
    public int skillChargeProgress;
    #endregion

    public SaveData()
    {
        playerHealth = 200;
        levelNumber = 0;
        checkpointNumber = 0;
        locationName = "";
        skillHealProgress = 0;
        skillHealthRegenProgeess = 0;
        skillChargeProgress = 0;
    }

    public SaveData(GameObject player, SceneController scene, GameObject[] skills)
    {
        playerHealth = player.GetComponent<StatsController>().health;
        levelNumber = scene.levelNumber;
        checkpointNumber = scene.lastCheckpoint.GetComponent<CheckpointController>().checkpointNumber;
        locationName = scene.lastCheckpoint.GetComponent<CheckpointController>().locationName;
        skillHealProgress = skills[0].GetComponent<SkillHeal>().sumOfHealingDone;
        skillHealthRegenProgeess = skills[1].GetComponent<SkillHPRegen>().sumOfHealthRegen;
        skillChargeProgress = skills[2].GetComponent<SkillCharge>().sumOfEnemiesHit;
    }
}
