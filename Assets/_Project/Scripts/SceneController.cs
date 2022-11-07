using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public GameObject player;
    private GameObject[] checkpoints;
    private GameObject startingPoint;
    public GameObject lastCheckpoint;
    private int startingPointsCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

        foreach (GameObject checkpoint in checkpoints)
        {
            if (checkpoint.GetComponent<CheckpointController>().isStartingPoint)
            {
                startingPoint = checkpoint;
                startingPointsCounter++;
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckpointReached(GameObject checkpoint)
    {
        lastCheckpoint = checkpoint;
    }

    public void RespawnPlayer()
    {
        Instantiate(player, lastCheckpoint.transform.position, Quaternion.identity);
    }
}
