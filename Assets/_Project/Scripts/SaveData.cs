using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public int levelNumber;
    public int checkpointNumber;

    #region Skills mastering
    public float skillHealProgress;
    #endregion

    public SaveData()
    {
        playerHealth = 200;
        levelNumber = 0;
        checkpointNumber = 0;
        skillHealProgress = 0;
    }

    public SaveData(GameObject player, SceneController scene, GameObject[] skills)
    {
        playerHealth = player.GetComponent<StatsController>().health;
        levelNumber = scene.levelNumber;
        checkpointNumber = scene.lastCheckpoint.GetComponent<CheckpointController>().checkpointNumber;
        skillHealProgress = skills[0].GetComponent<SkillHeal>().sumOfHealingDone;
    }
}
