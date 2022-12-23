using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public int[] playerSkills = new int[3];
    public int[] unlockedSkills;
    public int levelNumber;
    public int checkpointNumber;
    public string locationName;
    public int chosenSkillSlot;

    #region Skills mastering 
    public float skillHealProgress;
    public float skillHealthRegenProgeess;
    public int skillChargeProgress;
    #endregion

    public SaveData()
    {
        playerHealth = 200;
        playerSkills[0] = -1;
        playerSkills[1] = -1;
        playerSkills[2] = -1;
        unlockedSkills = new int[0];
        levelNumber = 0;
        checkpointNumber = 0;
        locationName = "";
        chosenSkillSlot = 0;
        skillHealProgress = 0;
        skillHealthRegenProgeess = 0;
        skillChargeProgress = 0;
    }

    public SaveData(GameObject player, SceneController scene, GameObject[] skills, GameManager gameManager)
    {
        playerHealth = player.GetComponent<StatsController>().health;
        playerSkills[0] = Array.IndexOf(skills, player.GetComponent<PlayerController>().skills[0]);
        playerSkills[1] = Array.IndexOf(skills, player.GetComponent<PlayerController>().skills[1]);
        playerSkills[2] = Array.IndexOf(skills, player.GetComponent<PlayerController>().skills[2]);
        unlockedSkills = gameManager.unlockedSkills;
        levelNumber = scene.levelNumber;
        checkpointNumber = scene.lastCheckpoint.GetComponent<CheckpointController>().checkpointNumber;
        locationName = scene.lastCheckpoint.GetComponent<CheckpointController>().locationName;
        chosenSkillSlot = player.GetComponent<PlayerController>().chosenSkillSlot;
        skillHealProgress = skills[0].GetComponent<SkillHeal>().sumOfHealingDone;
        skillHealthRegenProgeess = skills[1].GetComponent<SkillHPRegen>().sumOfHealthRegen;
        skillChargeProgress = skills[2].GetComponent<SkillCharge>().sumOfEnemiesHit;
    }
}
