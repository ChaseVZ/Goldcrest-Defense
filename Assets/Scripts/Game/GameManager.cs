using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    AudioSource buyPhaseMusic;
    AudioSource attackPhaseMusic;
    AudioSource gameOverMusic;
    AudioSource bossPhaseMusic;
    AudioSource waveWinChime;

    WaveSpawner waveSpawner;

    /* must be 0 - 1*/
    //public float buyMusicVolume;
    //public float atkMusicVolume;
    //public float bossMusicVolume;

    [SerializeField] string masterVolume;
    [SerializeField] string musicVolume;

    public bool tutorialMode = false;
    public bool blockUpgrades = false;
    private bool buyPhase = true;
    private bool atkPhase = false;
    private bool bossRound = false;

    bool gameOver;
    [SerializeField] float fadeTime;

    [SerializeField] GameObject x1_speed_button;
    [SerializeField] GameObject x2_speed_button;
    [SerializeField] Abilities abilitiesGO;

    public GameObject Trader;
    public int traderFrequency = 1;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        // Audio Components
        buyPhaseMusic = GameObject.Find("BuyPhase").GetComponent<AudioSource>();
        bossPhaseMusic = GameObject.Find("BossPhase").GetComponent<AudioSource>();
        attackPhaseMusic = GameObject.Find("AttackPhase").GetComponent<AudioSource>();
        gameOverMusic = GameObject.Find("GameOverAudio").GetComponent<AudioSource>();
        waveWinChime = GameObject.Find("WaveWin").GetComponent<AudioSource>();

        buyPhaseMusic.volume = 0;
        bossPhaseMusic.volume = 0;
        attackPhaseMusic.volume = 0;

        // Script Components
        waveSpawner = GameObject.FindGameObjectWithTag("GameManager").GetComponent<WaveSpawner>();

        gameOver = false;

        buyModeBegin();
    }

    public void ShowTrader()
    {
        Trader.SetActive(true);
    }

    public void HideTrader()
    {
        Trader.SetActive(false);
    }

    public bool TraderActive()
    {
        return Trader.activeSelf;
    }

    public void buyModeBegin()
    {
        if (gameOver) { return; }

        buyPhase = true;
        atkPhase = false;
        bossRound = false;

        int waveNum = waveSpawner.GetWaveNumber();
        if (waveNum != 0 && waveNum != 1 && !waveSpawner.GetBossDefeated())
        {
            waveWinChime.Play();
        }

        StopAllCoroutines();
        if (attackPhaseMusic.isPlaying) { StartCoroutine(StartFade(attackPhaseMusic, fadeTime-1f, 0f)); }
        if (bossPhaseMusic.isPlaying) { StartCoroutine(StartFade(bossPhaseMusic, fadeTime-1f, 0f)); }

        if (!buyPhaseMusic.isPlaying) { buyPhaseMusic.Play(); }
        else { buyPhaseMusic.UnPause(); }

        StartCoroutine(StartFade(buyPhaseMusic, fadeTime, PlayerPrefs.GetFloat(musicVolume) * (PlayerPrefs.GetFloat(masterVolume))));

        abilitiesGO.setAbilitiesReady = true; // reset cooldowns to zero on buy phase


        if (waveNum % traderFrequency == 0)
            ShowTrader();
    }

    public void attackModeBegin()
    {
        if (gameOver) { return; }

        int waveNumber = gameObject.GetComponent<WaveSpawner>().GetWaveNumber();

        StopAllCoroutines();
        if (buyPhaseMusic.isPlaying) { StartCoroutine(StartFade(buyPhaseMusic, fadeTime-1f, 0f)); }
        if (Array.Exists(waveSpawner.MiniBossWaveNumbers, wave => wave == waveNumber))
        { 
            bossRound = true;
            atkPhase = true;
            buyPhase = false;

            bossPhaseMusic.Play();
            StartCoroutine(StartFade(bossPhaseMusic, fadeTime, PlayerPrefs.GetFloat(musicVolume) * PlayerPrefs.GetFloat(masterVolume)));
        }
        else
        {
            bossRound = false;
            atkPhase = true;
            buyPhase = false;

            attackPhaseMusic.Play();
            StartCoroutine(StartFade(attackPhaseMusic, fadeTime, PlayerPrefs.GetFloat(musicVolume) * PlayerPrefs.GetFloat(masterVolume)));
        }

        HideTrader();
    }

    // stop everything and change volume
    public void updateVolume()
    {
        StopAllCoroutines();
        if (buyPhase) { buyPhaseMusic.volume = PlayerPrefs.GetFloat(musicVolume) * PlayerPrefs.GetFloat(masterVolume); }
        else if (atkPhase && !bossRound) { attackPhaseMusic.volume = PlayerPrefs.GetFloat(musicVolume) * PlayerPrefs.GetFloat(masterVolume); }
        else if (bossRound) { bossPhaseMusic.volume = PlayerPrefs.GetFloat(musicVolume) * PlayerPrefs.GetFloat(masterVolume); } 
    }

    // add extra lives?
    // call game over screen
    public void healthDepleted()
    {
        buyPhaseMusic.Stop();
        attackPhaseMusic.Stop();     
        bossPhaseMusic.Stop();
        gameOverMusic.Play();

        gameOver = true;
    }

    public bool getBossDefeated() { return waveSpawner.GetBossDefeated(); }
    public bool isInBuyPhase() { return buyPhase; }
    public bool isInAttackPhase() { return atkPhase; }
    public bool isInBossRound() { return bossRound; }

    /* FadeTime = seconds */
    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (targetVolume == 0f) { audioSource.Pause(); }
        yield break;
    }
    
    public void x1_speed() { Time.timeScale = 2f; x1_speed_button.SetActive(false); x2_speed_button.SetActive(true); }
    public void x2_speed() { Time.timeScale = 1f; x1_speed_button.SetActive(true); x2_speed_button.SetActive(false); }
}
