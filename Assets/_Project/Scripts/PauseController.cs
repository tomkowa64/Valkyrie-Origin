using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PauseController
{
    public static bool gameIsPaused = false;
    public static bool choosingSkills = false;

    public static void Pause()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public static void Resume()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
    }
}
