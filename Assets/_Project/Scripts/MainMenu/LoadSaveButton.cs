using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class LoadSaveButton : MonoBehaviour
{
    #region Variables
    [Header("Do not touch")]
    private List<FileInfo> saves;
    private GameManager gameManager;
    private bool active;
    public string saveName;
    private SaveData saveData;

    [Header("To fill")]
    public int saveNumber;
    public GameObject deletePrompt;
    private GameObject activePrompt;

    [Header("Button components")]
    public GameObject image;
    public GameObject nameOfSave;
    public GameObject numberOfLevel;
    public GameObject location;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        ReloadSavesData();
    }

    public void ReloadSavesData()
    {
        saves = SaveSystem.GetAllSaves();

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

    public void DeleteSave()
    {
        activePrompt = Instantiate(deletePrompt, new Vector3(0f, 0f, 0f), Quaternion.identity);
        activePrompt.transform.SetParent(gameObject.transform.parent);
        activePrompt.transform.SetPositionAndRotation(gameObject.transform.parent.position, gameObject.transform.parent.rotation);
        activePrompt.GetComponent<DeleteSavePrompt>().savePath = saves[saveNumber - 1].FullName;
        activePrompt.GetComponent<DeleteSavePrompt>().text.GetComponent<TextMeshProUGUI>().text = "Are you sure you want to delete save " + saveName;
    }
}
