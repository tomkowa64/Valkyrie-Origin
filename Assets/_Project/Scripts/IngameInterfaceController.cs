using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameInterfaceController : MonoBehaviour
{
    #region Variables
    private PlayerController playerController;
    private StatsController playerStats;
    public Slider healthBar;
    public Slider manaBar;
    public Slider staminaBar;

    #region Skills
    public Image skillOneImage;
    public Image skillTwoImage;
    public Image skillThreeImage;
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

        skillOneImage.sprite = playerController.skills[0].GetComponent<Image>().sprite;
        skillTwoImage.sprite = playerController.skills[1].GetComponent<Image>().sprite;
        skillThreeImage.sprite = playerController.skills[2].GetComponent<Image>().sprite;
    }
}
