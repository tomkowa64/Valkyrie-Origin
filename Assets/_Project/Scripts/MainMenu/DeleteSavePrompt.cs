using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeleteSavePrompt : MonoBehaviour
{
    [Header("Do not touch")]
    public string savePath;
    private GameObject[] saveButtons;

    [Header("To fill")]
    public GameObject text;
    private Scene scene;

    public void ClosePrompt()
    {
        Destroy(gameObject);
    }

    public void Delete()
    {
        saveButtons = GameObject.FindGameObjectsWithTag("SaveButton");
        File.Delete(savePath);

        foreach(GameObject button in saveButtons)
        {
            button.GetComponent<LoadSaveButton>().ReloadSavesData();
        }

        ClosePrompt();
    }
}
