using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ChooseSkillPanel : MonoBehaviour
{
    #region Variables
    public GameObject[] activeSkills;
    private Image[] activeSkillsImages = new Image[3];
    public GameObject[] chosenSkillBorders;
    private GameObject[] allSkills;
    private GameManager gameManager;
    private PlayerController player;
    private int clickedSkillSlot;

    #region Text
    public GameObject skillName;
    public GameObject skillDesc;
    #endregion
    #endregion

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        for (int i = 0; i < 3; i++)
        {
            activeSkills[i].TryGetComponent<Image>(out activeSkillsImages[i]);

            if (player.skills[i] != null)
            {
                activeSkillsImages[i].sprite = player.skills[i].GetComponent<Image>().sprite;
                activeSkillsImages[i].color = player.skills[i].GetComponent<Image>().color;
            }
        }

        allSkills = GameObject.FindGameObjectsWithTag("SkillSelectionSkills");
        Array.Sort(gameManager.unlockedSkills);

        for (int i = 0; i < gameManager.unlockedSkills.Length; i++)
        {
            allSkills[i].GetComponent<Image>().sprite = gameManager.skills[gameManager.unlockedSkills[i]].GetComponent<Image>().sprite;
            allSkills[i].GetComponent<Image>().color = gameManager.skills[gameManager.unlockedSkills[i]].GetComponent<Image>().color;
        }

        skillName.GetComponent<TextMeshProUGUI>().text = "";
        skillDesc.GetComponent<TextMeshProUGUI>().text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PauseController.choosingSkills)
        {
            BackToGame();
        }
    }

    public void ChooseSkillSlot(int slotNumber)
    {
        clickedSkillSlot = slotNumber;

        for (int i = 0; i < chosenSkillBorders.Length; i++)
        {
            chosenSkillBorders[i].SetActive(false);
        }

        chosenSkillBorders[slotNumber].SetActive(true);
    }

    public void StartSkillHover(int slotNumber)
    {
        if (slotNumber < gameManager.unlockedSkills.Length)
        {
            skillName.GetComponent<TextMeshProUGUI>().text = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<SkillController>().skillName;
            skillDesc.GetComponent<TextMeshProUGUI>().text = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<SkillController>().skillDescription;
        }
    }

    public void StopSkillHover()
    {
        skillName.GetComponent<TextMeshProUGUI>().text = "";
        skillDesc.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SelectSkill(int slotNumber)
    {
        if (slotNumber < gameManager.unlockedSkills.Length)
        {
            int position = Array.IndexOf(player.skills, gameManager.skills[gameManager.unlockedSkills[slotNumber]]);

            if (position > -1)
            {
                player.skills[position] = player.skills[clickedSkillSlot];
                activeSkillsImages[position].sprite = player.skills[clickedSkillSlot].GetComponent<Image>().sprite;
                activeSkillsImages[position].color = player.skills[clickedSkillSlot].GetComponent<Image>().color;
                player.skills[clickedSkillSlot] = gameManager.skills[gameManager.unlockedSkills[slotNumber]];
                activeSkillsImages[clickedSkillSlot].sprite = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<Image>().sprite;
                activeSkillsImages[clickedSkillSlot].color = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<Image>().color;
            }
            else
            {
                player.skills[clickedSkillSlot] = gameManager.skills[gameManager.unlockedSkills[slotNumber]];
                activeSkillsImages[clickedSkillSlot].sprite = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<Image>().sprite;
                activeSkillsImages[clickedSkillSlot].color = gameManager.skills[gameManager.unlockedSkills[slotNumber]].GetComponent<Image>().color;
            }

            if (player.chosenSkillSlot > 0)
            {
                player.chosenSkill = player.skills[player.chosenSkillSlot - 1];
            }
        }
    }

    public void BackToGame()
    {
        SaveSystem.Save(gameManager.saveName, GameObject.FindGameObjectWithTag("Player"), GameObject.FindGameObjectWithTag("GameController").GetComponent<SceneController>(), gameManager.skills, gameManager, GameObject.FindGameObjectWithTag("GameController").GetComponent<Level0Controller>());
        SceneManager.UnloadSceneAsync("SkillSelection");
        PauseController.choosingSkills = false;
        PauseController.Resume();
    }
}
