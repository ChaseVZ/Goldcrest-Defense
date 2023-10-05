using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{

    bool gameover = false;
    public void ShowScreen()
    {
        if (!gameover)
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
            gameover = true;
        }
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
