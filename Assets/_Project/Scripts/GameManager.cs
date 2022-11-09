using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public GameObject playerPrefab;
    public string saveName;
    public SaveData saveData;

    #region Skills

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        saveData = SaveSystem.Load(saveName);

        if (saveData.levelNumber == -1)
        {
            SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadTestField()
    {
        saveName = "test";
        saveData = SaveSystem.Load(saveName);
        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }
}
