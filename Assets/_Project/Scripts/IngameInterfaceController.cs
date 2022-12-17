using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class IngameInterfaceController : MonoBehaviour
{
    #region Variables
    private PlayerController playerController;
    private StatsController playerStats;
    public GameObject pauseMenu;

    [Header("Status bars")]
    public Slider healthBar;
    public Slider manaBar;
    public Slider staminaBar;

    #region Skills
    #region Images
    [Header("Skill images")]
    public Image skillOneImage;
    public Image skillTwoImage;
    public Image skillThreeImage;
    #endregion

    #region Timers
    [Header("Skill timers")]
    public GameObject skillOneCDTimer;
    public GameObject skillTwoCDTimer;
    public GameObject skillThreeCDTimer;
    #endregion

    #region Keys
    [Header("Skill hotkeys")]
    public GameObject skillOneHotKey;
    public GameObject skillTwoHotKey;
    public GameObject skillThreeHotKey;
    #endregion

    #region Choosen border
    [Header("Skill choosen borders")]
    public GameObject skillOneBorder;
    public GameObject skillTwoBorder;
    public GameObject skillThreeBorder;
    #endregion

    #region Loading bars
    [Header("Skill loading bar")]
    public Slider skillOneBar;
    public Slider skillTwoBar;
    public Slider skillThreeBar;
    #endregion
    #endregion
    #endregion

    // Update is called once per frame
    void Update()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        healthBar.minValue = 0f;
        healthBar.maxValue = playerStats.maxHealth;
        manaBar.minValue = 0f;
        manaBar.maxValue = playerStats.maxMana;
        staminaBar.minValue = 0f;
        staminaBar.maxValue = playerStats.maxStamina;
        healthBar.value = playerStats.health;
        manaBar.value = playerStats.mana;
        staminaBar.value = playerStats.stamina;
        skillOneBar.interactable = false;
        skillTwoBar.interactable = false;
        skillThreeBar.interactable = false;

        if (skillOneBar.value == 0f)
        {
            skillOneBar.gameObject.SetActive(false);
        }
        else
        {
            skillOneBar.gameObject.SetActive(true);
        }

        if (skillTwoBar.value == 0f)
        {
            skillTwoBar.gameObject.SetActive(false);
        }
        else
        {
            skillTwoBar.gameObject.SetActive(true);
        }

        if (skillThreeBar.value == 0f)
        {
            skillThreeBar.gameObject.SetActive(false);
        }
        else
        {
            skillThreeBar.gameObject.SetActive(true);
        }

        if (playerController.skills[0] != null)
        {
            skillOneImage.sprite = playerController.skills[0].GetComponent<Image>().sprite;
            skillOneImage.color = playerController.skills[0].GetComponent<Image>().color;
            skillOneBar.value = playerController.skills[0].GetComponent<SkillController>().loadingProgress;

            if (playerController.skills[0].GetComponent<SkillController>().onCooldown)
            {
                skillOneCDTimer.SetActive(true);
                skillOneCDTimer.GetComponent<TextMeshProUGUI>().text =
                    Mathf.RoundToInt(
                        playerController.skills[0].GetComponent<SkillController>().cooldownTime -
                        playerController.skills[0].GetComponent<SkillController>().cdTimer
                    ).ToString();
            }
            else
            {
                skillOneCDTimer.SetActive(false);
            }
        }
        else
        {
            skillOneBar.value = 0f;
        }

        if (playerController.skills[1] != null)
        {
            skillTwoImage.sprite = playerController.skills[1].GetComponent<Image>().sprite;
            skillTwoImage.color = playerController.skills[1].GetComponent<Image>().color;
            skillTwoBar.value = playerController.skills[1].GetComponent<SkillController>().loadingProgress;

            if (playerController.skills[1].GetComponent<SkillController>().onCooldown)
            {
                skillTwoCDTimer.SetActive(true);
                skillTwoCDTimer.GetComponent<TextMeshProUGUI>().text =
                    Mathf.RoundToInt(
                        playerController.skills[1].GetComponent<SkillController>().cooldownTime -
                        playerController.skills[1].GetComponent<SkillController>().cdTimer
                    ).ToString();
            }
            else
            {
                skillTwoCDTimer.SetActive(false);
            }
        }
        else
        {
            skillTwoBar.value = 0f;
        }

        if (playerController.skills[2] != null)
        {
            skillThreeImage.sprite = playerController.skills[2].GetComponent<Image>().sprite;
            skillThreeImage.color = playerController.skills[2].GetComponent<Image>().color;
            skillThreeBar.value = playerController.skills[2].GetComponent<SkillController>().loadingProgress;

            if (playerController.skills[2].GetComponent<SkillController>().onCooldown)
            {
                skillThreeCDTimer.SetActive(true);
                skillThreeCDTimer.GetComponent<TextMeshProUGUI>().text =
                    Mathf.RoundToInt(
                        playerController.skills[2].GetComponent<SkillController>().cooldownTime -
                        playerController.skills[2].GetComponent<SkillController>().cdTimer
                    ).ToString();
            }
            else
            {
                skillThreeCDTimer.SetActive(false);
            }
        }
        else
        {
            skillThreeBar.value = 0f;
        }

        switch (playerController.chosenSkillSlot)
        {
            case 1:
                skillOneBorder.SetActive(true);
                skillTwoBorder.SetActive(false);
                skillThreeBorder.SetActive(false);
                break;

            case 2:
                skillOneBorder.SetActive(false);
                skillTwoBorder.SetActive(true);
                skillThreeBorder.SetActive(false);
                break;

            case 3:
                skillOneBorder.SetActive(false);
                skillTwoBorder.SetActive(false);
                skillThreeBorder.SetActive(true);
                break;

            default:
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseController.gameIsPaused)
            {
                pauseMenu.SetActive(false);

                PauseController.Resume();
            }
            else
            {
                PauseController.Pause();
                pauseMenu.SetActive(true);
            }
        }
    }

    public void LoadOptionsMenu()
    {
        Debug.Log("Show options menu");
    }

    public void QuitToMenu()
    {
        PauseController.Resume();
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
}
