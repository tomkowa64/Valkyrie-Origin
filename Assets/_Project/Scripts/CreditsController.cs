using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    private float timer;

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        if (timer >= 15f)
        {
            SceneManager.LoadScene("startMenu", LoadSceneMode.Single);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
