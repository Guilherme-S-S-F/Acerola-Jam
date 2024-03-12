using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jam.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameRules : MonoBehaviour
{
    [SerializeField] GameObject finishedMenu;
    [SerializeField] GameObject jumpscare;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI tittle;
    [SerializeField] TextMeshProUGUI scoreLbl;
    [Header("Scores")]
    private int missionItemCount = 0;
    public GameObject[] collectablesMission;

    [Header("Flags")]
    public bool gameWin = false;
    public bool isPause = false;


    public void Start()
    {
        GameScoreEvent.PlayerScored += playerScored;
        EnemyAttackEvent.GameOver += gameOver;
        finishedMenu.SetActive(false);
        jumpscare.SetActive(false);
        isPause = true;
    }

    private void Update()
    {
        if (gameWin)
            Won();
    }
    public void goToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
    void Won()
    {
        gameWin = false;
        isPause = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        finishedMenu.SetActive(true);
    }
    private void gameOver()
    {
        isPause = true;
        
        jumpscare.SetActive(true);
        jumpscare.GetComponent<AudioSource>().Play();

        StartCoroutine(stopJumpScare());

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
    }

    IEnumerator stopJumpScare()
    {
        yield return new WaitForSeconds(2);
        jumpscare.SetActive(false);
        finishedMenu.SetActive(true);

        tittle.text = "GAME OVER";
        description.text = "You lost your soul to the monster.";
    }

    public void playerScored()
    {
        missionItemCount++;

        scoreLbl.text = missionItemCount + "/10 fragments";

        if (missionItemCount >= collectablesMission.Length)
        {
            gameWin = true;
        }
    }
}
