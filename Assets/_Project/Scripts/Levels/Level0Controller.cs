using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Level0Controller : MonoBehaviour
{
    #region Variables
    public GameObject boss;
    private GameManager gameManager;
    private GoblinBossController bossController;
    [SerializeField] private GameObject bossFightWallCanvas;
    [SerializeField] private TextMeshProUGUI killedEnemiesText;
    [SerializeField] private TextMeshProUGUI enemiesToKillText;
    [SerializeField] private int enemiesToKill;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private LayerMask player;

    [Header ("Do not touch")]
    public int killedEnemies;
    #endregion

    private void Start()
    {
        bossController = boss.GetComponent<GoblinBossController>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemiesToKillText.text = enemiesToKill.ToString();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        killedEnemies = gameManager.saveData.level0KilledEnemies;
    }
    
    void Update()
    {
        if (!PauseController.gameIsPaused)
        {
            #region Killed Enemies
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            killedEnemiesText.text = killedEnemies.ToString();

            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<StatsController>().health <= 0)
                {
                    killedEnemies++;
                }
            }
            #endregion

            #region Boss Fight Wall
            if (bossController.bossFightStarted)
            {
                bossController.bossFightWall.SetActive(true);
            }
            else
            {
                if (killedEnemies < enemiesToKill)
                {
                    bossController.bossFightWall.SetActive(true);

                    if (PlayerNextToBossFightWall())
                    {
                        bossFightWallCanvas.SetActive(true);
                    }
                }
                else
                {
                    bossController.bossFightWall.SetActive(false);
                }
            }

            if (!PlayerNextToBossFightWall())
            {
                bossFightWallCanvas.SetActive(false);
            }
            #endregion

            #region Boss killed
            if (boss.GetComponent<StatsController>().health <= 0)
            {
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
            }
            #endregion
        }
    }

    private bool PlayerNextToBossFightWall()
    {
        BoxCollider2D wallCollider = bossController.bossFightWall.GetComponent<BoxCollider2D>();

        return Physics2D.BoxCast(wallCollider.bounds.center, wallCollider.bounds.size, 0f, Vector2.left, .1f, player);
    }
}
