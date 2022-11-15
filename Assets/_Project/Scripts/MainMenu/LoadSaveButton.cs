using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class LoadSaveButton : MonoBehaviour
{
    private List<FileInfo> saves;
    private GameManager gameManager;
    private bool active;
    public int saveNumber;
    public string saveName;
    private SaveData saveData;

    [Header("Button components")]
    public GameObject image;
    public GameObject nameOfSave;
    public GameObject numberOfLevel;
    public GameObject location;

    // Start is called before the first frame update
    void Start()
    {
        saves = SaveSystem.GetAllSaves();
        Debug.Log(saves.Count);

        if (saves.Count >= saveNumber)
        {
            saveName = saves[saveNumber - 1].Name.Split('.')[0];
            saveData = SaveSystem.Load(saveName);
            nameOfSave.GetComponent<TextMeshProUGUI>().text = saveName;
            numberOfLevel.GetComponent<TextMeshProUGUI>().text = saveData.levelNumber.ToString();
            location.GetComponent<TextMeshProUGUI>().text = saveData.locationName;
            active = true;
        }
        else
        {
            nameOfSave.GetComponent<TextMeshProUGUI>().text = "Empty Slot";
            numberOfLevel.GetComponent<TextMeshProUGUI>().text = "";
            location.GetComponent<TextMeshProUGUI>().text = "";
            image.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
            active = false;
        }
    }

    public void LoadSave()
    {
        if (active)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            gameManager.saveName = saveName;
            gameManager.LoadGame();
        }
    }
}
