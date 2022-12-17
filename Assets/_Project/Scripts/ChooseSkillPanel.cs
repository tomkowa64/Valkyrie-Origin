using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseSkillPanel : MonoBehaviour
{
    public void BackToGame()
    {
        SceneManager.UnloadSceneAsync("ChooseOfSkillsPanel");
        PauseController.Resume();
    }
}
