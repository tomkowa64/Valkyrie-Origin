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

        activeSkills[0].TryGetComponent<Image>(out activeSkillsImages[0]);
        activeSkills[1].TryGetComponent<Image>(out activeSkillsImages[1]);
        activeSkills[2].TryGetComponent<Image>(out activeSkillsImages[2]);

        activeSkillsImages[0].sprite = player.skills[0].GetComponent<Image>().sprite;
        activeSkillsImages[0].color = player.skills[0].GetComponent<Image>().color;
        activeSkillsImages[1].sprite = player.skills[1].GetComponent<Image>().sprite;
        activeSkillsImages[1].color = player.skills[1].GetComponent<Image>().color;
        activeSkillsImages[2].sprite = player.skills[2].GetComponent<Image>().sprite;
        activeSkillsImages[2].color = player.skills[2].GetComponent<Image>().color;

        allSkills = GameObject.FindGameObjectsWithTag("SkillSelectionSkills");

        for (int i = 0; i < gameManager.skills.Length; i++)
        {
            allSkills[i].GetComponent<Image>().sprite = gameManager.skills[i].GetComponent<Image>().sprite;
            allSkills[i].GetComponent<Image>().color = gameManager.skills[i].GetComponent<Image>().color;
        }

        skillName.GetComponent<TextMeshProUGUI>().text = "";
        skillDesc.GetComponent<TextMeshProUGUI>().text = "";
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
        if (slotNumber < gameManager.skills.Length)
        {
            skillName.GetComponent<TextMeshProUGUI>().text = gameManager.skills[slotNumber].GetComponent<SkillController>().skillName;
            skillDesc.GetComponent<TextMeshProUGUI>().text = gameManager.skills[slotNumber].GetComponent<SkillController>().skillDescription;
        }
    }

    public void StopSkillHover()
    {
        skillName.GetComponent<TextMeshProUGUI>().text = "";
        skillDesc.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SelectSkill(int slotNumber)
    {
        if (slotNumber < gameManager.skills.Length)
        {
            int position = Array.IndexOf(player.skills, gameManager.skills[slotNumber]);

            if (position > -1)
            {
                player.skills[position] = player.skills[clickedSkillSlot];
                activeSkillsImages[position].sprite = player.skills[clickedSkillSlot].GetComponent<Image>().sprite;
                activeSkillsImages[position].color = player.skills[clickedSkillSlot].GetComponent<Image>().color;
                player.skills[clickedSkillSlot] = gameManager.skills[slotNumber];
                activeSkillsImages[clickedSkillSlot].sprite = gameManager.skills[slotNumber].GetComponent<Image>().sprite;
                activeSkillsImages[clickedSkillSlot].color = gameManager.skills[slotNumber].GetComponent<Image>().color;
            }
            else
            {
                player.skills[clickedSkillSlot] = gameManager.skills[slotNumber];
                activeSkillsImages[clickedSkillSlot].sprite = gameManager.skills[slotNumber].GetComponent<Image>().sprite;
                activeSkillsImages[clickedSkillSlot].color = gameManager.skills[slotNumber].GetComponent<Image>().color;
            }

            player.chosenSkill = player.skills[player.chosenSkillSlot - 1];
        }
    }

    public void BackToGame()
    {
        SceneManager.UnloadSceneAsync("SkillSelection");
        PauseController.Resume();
    }
}
