using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu2 : MonoBehaviour
{
    public static bool GameIsPaused = false;

    private Toggle toggle;

    private void Start()
    {
        toggle = gameObject.GetComponentInChildren<Toggle>();
    }
    void Update()
    {
        /* only pause if all menus are closed */
        if (Input.GetKeyDown(KeyCode.Escape) && MenusAreClosed())
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    bool MenusAreClosed()
    {
        /* menus are open */
        if(BuildManager.instance.getPurchaseMode())
        {
            BuildManager.instance.closeAllMenus();
            return false;
        }
        return true;
    }

    void Pause()
    {
        //Cursor.lockState = CursorLockMode.None;
        gameObject.SetActive(true);
        Time.timeScale = 0f;    /* Pause time */
        GameIsPaused = true;
    }

    public void HaungsMode()
    {
        if (toggle.isOn == true)
        {
            Debug.Log("Haungs Mode ON");
        }
        else if (toggle.isOn == false)
        {
            Debug.Log("Haungs Mode OFF");
        }
    }

    public void Resume()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }


}
