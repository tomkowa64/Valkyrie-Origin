using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public int levelNumber;
    public int checkpointNumber;

    public SaveData (GameObject player, SceneController scene)
    {
        playerHealth = player.GetComponent<StatsController>().health;
        levelNumber = scene.levelNumber;
        checkpointNumber = scene.lastCheckpoint.GetComponent<CheckpointController>().checkpointNumber;
    }
}
