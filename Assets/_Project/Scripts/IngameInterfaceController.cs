using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngameInterfaceController : MonoBehaviour
{
    #region Variables
    private PlayerController playerController;
    private StatsController playerStats;

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
    #endregion
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatsController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        healthBar.minValue = 0f;
        healthBar.maxValue = playerStats.maxHealth;
        manaBar.minValue = 0f;
        manaBar.maxValue = playerStats.maxMana;
        staminaBar.minValue = 0f;
        staminaBar.maxValue = playerStats.maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = playerStats.health;
        manaBar.value = playerStats.mana;
        staminaBar.value = playerStats.stamina;

        if (playerController.skills[0] != null)
        {
            skillOneImage.sprite = playerController.skills[0].GetComponent<Image>().sprite;

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

        if (playerController.skills[1] != null)
        {
            skillTwoImage.sprite = playerController.skills[1].GetComponent<Image>().sprite;

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

        if (playerController.skills[2] != null)
        {
            skillThreeImage.sprite = playerController.skills[2].GetComponent<Image>().sprite;

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
    }
}
