using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public int checkpointNumber;
    public string locationName;
    public bool isStartingPoint = false;
    public bool canInteract = false;

    private void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (Input.GetKeyDown(KeyCode.F) && canInteract)
            {
                PauseController.Pause();
                PauseController.choosingSkills = true;
                SceneManager.LoadScene("SkillSelection", LoadSceneMode.Additive);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>().CheckpointReached(gameObject);
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canInteract = false;
        }
    }
}
