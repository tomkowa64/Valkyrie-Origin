using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public int levelNumber;
    private GameManager gameManager;
    private GameObject playerPrefab;
    private GameObject player;
    private GameObject[] checkpoints;
    private GameObject startingPoint;
    public GameObject lastCheckpoint;
    private int startingPointsCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerPrefab = gameManager.playerPrefab;
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (GameObject checkpoint in checkpoints)
        {
            if (checkpoint.GetComponent<CheckpointController>().isStartingPoint)
            {
                startingPoint = checkpoint;
                startingPointsCounter++;
            }

            if (checkpoint.GetComponent<CheckpointController>().checkpointNumber == gameManager.saveData.checkpointNumber)
            {
                lastCheckpoint = checkpoint;
            }
        }

        if (startingPointsCounter != 1)
        {
            Debug.Log("Exactly 1 startring point is needed on the map!");
        }
        else
        {
            if (lastCheckpoint == null)
            {
                lastCheckpoint = startingPoint;
            }

            RespawnPlayer();
            player = GameObject.FindGameObjectWithTag("Player");
            LoadPlayerData();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckpointReached(GameObject checkpoint)
    {
        lastCheckpoint = checkpoint;
        SaveSystem.Save(gameManager.saveName, player, this, gameManager.skills);
    }

    public void RespawnPlayer()
    {
        Instantiate(playerPrefab, lastCheckpoint.transform.position, Quaternion.identity);
    }

    private void LoadPlayerData()
    {
        player.GetComponent<StatsController>().health = gameManager.saveData.playerHealth;
    }
}
