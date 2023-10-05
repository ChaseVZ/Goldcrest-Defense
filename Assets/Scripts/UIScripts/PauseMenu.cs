using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    [SerializeField] GameObject optionsMenu;

    public Toggle toggle;
    public TextMeshProUGUI sprite;
    public GameObject skipWave;

    private ResourceManager rm;
    private TownHealth th;

    bool godMode = false;
    bool changeColor = true;
    bool showGodMode = false;
    int[] konami_code = {273, 273, 274, 274, 276, 275, 276, 275, 97, 98, 13};
    int konami_code_idx = 0;

    float originalTimeScale = 1f;

    private void Start()
    {
        th = GameObject.FindGameObjectWithTag("GameManager").GetComponent<TownHealth>();
        rm = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        optionsMenu.SetActive(false);

    }

    void Update()
    {
        /* only pause if all menus are closed */
        if (Input.GetKeyDown(KeyCode.Escape) && MenusAreClosed())
        {
            if (GameIsPaused)
            {
                if (optionsMenu.activeInHierarchy) { optionsMenu.SetActive(false); pauseMenuUI.SetActive(true); }
                else if (pauseMenuUI.activeInHierarchy) { Resume(); }
            }
            else
            {
                Pause();
            }
        }

        if (godMode)
        {
            if (rm.resourceTotal < rm.maxResource)
            {
                rm.addResource(rm.maxResource);
            }

            float invincibleHealth = 100000f;
            th.setMaxHealth(invincibleHealth);
            th.AddHealth(invincibleHealth);

            if (changeColor && GameIsPaused) {StartCoroutine(colorChange());}
        }

        if (GameIsPaused && !showGodMode && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && konami_code[konami_code_idx] == (int)KeyCode.UpArrow) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && konami_code[konami_code_idx] == (int)KeyCode.DownArrow) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && konami_code[konami_code_idx] == (int)KeyCode.LeftArrow) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && konami_code[konami_code_idx] == (int)KeyCode.RightArrow) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.A) && konami_code[konami_code_idx] == (int)KeyCode.A) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.B) && konami_code[konami_code_idx] == (int)KeyCode.B) { konami_code_idx++; }
            else if (Input.GetKeyDown(KeyCode.Return) && konami_code[konami_code_idx] == (int)KeyCode.Return) { showGodMode = true; toggle.gameObject.SetActive(true); }
            else { konami_code_idx = 0; }
        }

    }

    //void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.isKey)
    //    {
    //        Debug.Log("Detected key code: " + e.keyCode);
    //    }
    //}

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
        originalTimeScale = Time.timeScale;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;    /* Pause time */
        GameIsPaused = true;
    }
    
    public void OptionsMenu()
    {

    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Scene cur = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cur.name);
    }

    public void Restart_toMainMenu()
    {
        Time.timeScale = 1f;
        //Scene cur = SceneManager.GetActiveScene();
        SceneManager.LoadScene("MainMenu");
    }

    public void Resume()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        konami_code_idx = 0;
        pauseMenuUI.SetActive(false);
        Time.timeScale = originalTimeScale;
        GameIsPaused = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void GodMode()
    {
        if (toggle.isOn == true)
        {
            godMode = true;
            skipWave.SetActive(true);

        }
        else if (toggle.isOn == false)
        {
            godMode = false;
            sprite.color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            skipWave.SetActive(false);
        }
    }

    IEnumerator colorChange()
    {
        changeColor = false;
        sprite.color = new Color(Random.value, Random.value, Random.value);
        yield return new WaitForSecondsRealtime(0.40f);  // works while timeScale = 0
        changeColor = true;
    }

}

