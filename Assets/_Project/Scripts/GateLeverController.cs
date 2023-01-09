using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateLeverController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject gate;
    [SerializeField] private GameObject[] gatePositions;
    [SerializeField] private Sprite[] leverSprites;
    private bool leverUp;
    [SerializeField] private float speed;
    public bool canInteract = false;
    #endregion

    void Start()
    {
        leverUp = true;
    }

    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            if (leverUp)
            {
                MoveGate(0);
            }
            else
            {
                MoveGate(1);
            }

            if (Input.GetKeyDown(KeyCode.F) && canInteract)
            {
                if (leverUp)
                {
                    leverUp = false;
                }
                else
                {
                    leverUp = true;
                }
            }
        }
    }

    private void MoveGate(int positionId)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = leverSprites[positionId];

        if (gate.transform.position != gatePositions[positionId].transform.position)
        {
            gate.transform.position = Vector3.MoveTowards(gate.transform.position, gatePositions[positionId].transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}
