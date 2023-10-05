using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCutscene : MonoBehaviour
{

    [SerializeField] GameObject ability1_scene;
    [SerializeField] GameObject info1;
    [SerializeField] GameObject ability2_scene;
    [SerializeField] GameObject info2;
    [SerializeField] GameObject ability3_scene;
    [SerializeField] GameObject info3;

    Animator RayAnimator;
    Animator CrystalAnimator;
    int isExit;
    int activeScene;
    bool cutsceneActive = false;

    [SerializeField] GameObject abilityGO;

    AudioSource reveal;
    AudioSource whoosh;

    WaveSpawner waveSpawner;

    void Start()
    {
        isExit = Animator.StringToHash("Exit");
        waveSpawner = GameObject.Find("GameManager").GetComponent<GameManager>().GetComponent<WaveSpawner>();

        reveal = GameObject.Find("Ability_Unlock_Audio").GetComponent<AudioSource>();
        whoosh = GameObject.Find("Ability_Unlock_Whoosh").GetComponent<AudioSource>();
    }

    public void unlocked1()
    {
        activeScene = 1;
        RayAnimator = ability1_scene.transform.GetChild(0).GetComponent<Animator>();
        CrystalAnimator = ability1_scene.transform.GetChild(1).GetComponent<Animator>();
        waveSpawner.setBossDefeated(true);
    }

    public void unlocked2()
    {
        activeScene = 2;
        RayAnimator = ability2_scene.transform.GetChild(0).GetComponent<Animator>();
        CrystalAnimator = ability2_scene.transform.GetChild(1).GetComponent<Animator>();
        waveSpawner.setBossDefeated(true);
    }

    public void unlocked3()
    {
        activeScene = 3;
        RayAnimator = ability3_scene.transform.GetChild(0).GetComponent<Animator>();
        CrystalAnimator = ability3_scene.transform.GetChild(1).GetComponent<Animator>();
        waveSpawner.setBossDefeated(true);
    }

    public void startCutscene()
    {
        if (!cutsceneActive)
        {
            if (activeScene == 1) { ability1_scene.SetActive(true); }
            else if (activeScene == 2) { ability2_scene.SetActive(true); }
            else if (activeScene == 3) { ability3_scene.SetActive(true); }
            reveal.Play();
            cutsceneActive = true;
        }
    }

    public void _loadInfo()
    {
        if (activeScene == 1) { info1.SetActive(true); }
        else if (activeScene == 2) { info2.SetActive(true); }
        else if (activeScene == 3) { info3.SetActive(true); }
    }

    void _unloadInfo()
    {
        if (activeScene == 1) { info1.SetActive(false); }
        else if (activeScene == 2) { info2.SetActive(false); }
        else if (activeScene == 3) { info3.SetActive(false); }
    }

    void _unloadCutscene()
    {
        if (activeScene == 1) { ability1_scene.SetActive(false); abilityGO.GetComponent<Abilities>()._set1(); }
        else if (activeScene == 2) { ability2_scene.SetActive(false); abilityGO.GetComponent<Abilities>()._set2(); }
        else if (activeScene == 3) { ability3_scene.SetActive(false); abilityGO.GetComponent<Abilities>()._set3(); }
    }

    public void _continue()
    {
        CrystalAnimator.SetBool(isExit, true);
        RayAnimator.SetBool(isExit, true);
        _unloadInfo();
        whoosh.Play();
    }

    public void _exitCutscene()
    {
        waveSpawner.setBossDefeated(false);
        _unloadCutscene();
        cutsceneActive = false;
    }

}
