using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Abilities : MonoBehaviour
{
    [Header("Ability 1")]
    public Image abilityImage1;
    public KeyCode ability1;
    public TextMeshProUGUI ability1Text;
    public TextMeshProUGUI keybind1Text;
    [SerializeField] float cooldown1 = 9f;
    [SerializeField] float ability1_firerate_magnitude = 2f;
    [SerializeField] float ability1_range = 15f;
    [Tooltip("Total time towers are boosted")]
    [SerializeField] float ability1_usetime = 5f;
    [SerializeField] bool unlockedAbility1;
    bool isCooldown1 = false;


    [Header("Ability 2")]
    public Image abilityImage2;
    public KeyCode ability2;
    public TextMeshProUGUI ability2Text;
    public TextMeshProUGUI keybind2Text;
    [SerializeField] float cooldown2 = 10f;
    [SerializeField] float ability2_slow_magnitude = 2f;
    [SerializeField] float ability2_range = 20f;
    [Tooltip("Total time enemies are slowed")]
    [SerializeField] float ability2_usetime = 5f;
    [SerializeField] bool unlockedAbility2;
    bool isCooldown2 = false;


    [Header("Ability 3")]
    public Image abilityImage3;
    public KeyCode ability3;
    public TextMeshProUGUI ability3Text;
    public TextMeshProUGUI keybind3Text;
    [SerializeField] float cooldown3 = 20f;
    [SerializeField] int ability3_boost_magnitude = 2;
    [SerializeField] float ability3_range = 300f; /* aka infinite */
    [Tooltip("Total time money is boosted")]
    [SerializeField] float ability3_usetime = 5f;
    [SerializeField] bool unlockedAbility3;
    bool isCooldown3 = false;

    GameObject player;
    GameObject resourceManager;
    GameObject gameManager;

    AudioSource audio1;
    AudioSource audio2;
    AudioSource audio3;

    public bool setAbilitiesReady = false; // called at beginning of buy phase


    private void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;
        ability1Text.text = "";
        ability2Text.text = "";
        ability3Text.text = "";
        unlockedAbility1 = false;
        unlockedAbility2 = false;
        unlockedAbility3 = false;
        keybind1Text.text = ability1.ToString();
        keybind2Text.text = ability2.ToString();
        keybind3Text.text = ability3.ToString();

        audio1 = GameObject.Find("Ability1_Audio").gameObject.GetComponent<AudioSource>();
        audio2 = GameObject.Find("Ability2_Audio").gameObject.GetComponent<AudioSource>();
        audio3 = GameObject.Find("Ability3_Audio").gameObject.GetComponent<AudioSource>();

        player = GameObject.FindWithTag("Player");
        resourceManager = GameObject.Find("ResourceManager");
        gameManager = GameObject.Find("GameManager");

        if (!unlockedAbility1) { abilityImage1.fillAmount = 1; }
        if (!unlockedAbility2) { abilityImage2.fillAmount = 1; }
        if (!unlockedAbility3) { abilityImage3.fillAmount = 1; }
    }

    private void Update()
    {
        if (unlockedAbility1 && isCooldown1 && setAbilitiesReady)
        {
            abilityImage1.fillAmount = 0;
        }

        if (unlockedAbility2 && isCooldown2 && setAbilitiesReady)
        {
            abilityImage2.fillAmount = 0;
        }

        if (unlockedAbility3 && isCooldown3 && setAbilitiesReady)
        {
            abilityImage3.fillAmount = 0;
        }

        setAbilitiesReady = false;


        Ability(unlockedAbility1, ability1, ref isCooldown1, abilityImage1, cooldown1, ability1Text, 1, keybind1Text);
        Ability(unlockedAbility2, ability2, ref isCooldown2, abilityImage2, cooldown2, ability2Text, 2, keybind2Text);
        Ability(unlockedAbility3, ability3, ref isCooldown3, abilityImage3, cooldown3, ability3Text, 3, keybind3Text);
    }

    public void _set1()
    {
        unlockedAbility1 = true;
        keybind1Text.gameObject.SetActive(true);
        abilityImage1.fillAmount = 0;
    }

    public void _set2()
    {
        unlockedAbility2 = true;
        keybind2Text.gameObject.SetActive(true);
        abilityImage2.fillAmount = 0;
    }

    public void _set3()
    {
        unlockedAbility3 = true;
        keybind3Text.gameObject.SetActive(true);
        abilityImage3.fillAmount = 0;
    }

    public void setUnlockAbility1()
    {
        gameManager.GetComponent<AbilityCutscene>().unlocked1();

    }
    public void setUnlockAbility2()
    {
        gameManager.GetComponent<AbilityCutscene>().unlocked2();

    }
    public void setUnlockAbility3()
    {
        gameManager.GetComponent<AbilityCutscene>().unlocked3();

    }

    public void upgradeAbility1()
    {
        ability1_firerate_magnitude += .25f;
        ability1_range += 4f;
        ability1_usetime += 2f;
        cooldown1 += 1f; /* optional: increase use time bc stronger (playtest) */
    }

    public void upgradeAbility2()
    {
        ability2_slow_magnitude += 0.25f;
        ability2_range += 4f;
        ability2_usetime += 2f;
        cooldown2 += 1f;
    }

    /* TODO: on upgrade, change multipler text */
    public void upgradeAbility3()
    {
        ability3_boost_magnitude += 1;
        ability2_usetime += 5f;
    }


    /* Red: Boost nearby tower firerate */
    void Ability1Function()
    {
        audio1.Play();

        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        player.GetComponent<PlayerController>().emitAbilityRing(ability1_range * 2, 1);
        foreach (GameObject tower in towers)
        {
            if (Vector3.Distance(tower.transform.position, player.transform.position) <= ability1_range)
            {
                tower.GetComponent<ITower>().boostFirerate(ability1_firerate_magnitude, ability1_usetime); 
            }
        }

    }

    /* Blue: slow enemies */
    void Ability2Function()
    {
        audio2.Play();
        StartCoroutine(ability2_delay(0.5f));
    }

    /* Green: money boost */
    void Ability3Function()
    {
        audio3.Play();
        StartCoroutine(ability3_delay(0.25f));

    }

    void Ability(bool unlocked, KeyCode key, ref bool isCooldown, Image abilityImage, float cooldown, TextMeshProUGUI text, float abilityNum, TextMeshProUGUI ktext)
    {
        if (!unlocked) { return; }
        if (Input.GetKey(key) && !isCooldown)
        {
            isCooldown = true;
            abilityImage.fillAmount = 1;
            ktext.gameObject.SetActive(false);
            if (abilityNum == 1) { Ability1Function(); }
            else if (abilityNum == 2) { Ability2Function(); }
            else if (abilityNum == 3) { Ability3Function(); }
        }
        if (isCooldown)
        {
            float timeElapsed = 1 / cooldown * Time.deltaTime;
            abilityImage.fillAmount -= timeElapsed;
            text.text = (Mathf.Round((abilityImage.fillAmount * cooldown))).ToString();

            if (abilityImage.fillAmount <= 0)
            {
                ktext.gameObject.SetActive(true);
                text.text = "";
                abilityImage.fillAmount = 0;
                isCooldown = false;
            }
        }
    }

    public void setAbilityReady()
    {

    }

    IEnumerator ability3_delay(float usetime)
    {
        yield return new WaitForSeconds(usetime);
        player.GetComponent<PlayerController>().emitAbilityRing(ability3_range * 2, 3);
        resourceManager.GetComponent<ResourceManager>().setMoneyBoost(ability3_boost_magnitude, ability3_usetime);
    }

    IEnumerator ability2_delay(float usetime)
    {
        yield return new WaitForSeconds(usetime);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        player.GetComponent<PlayerController>().emitAbilityRing(ability2_range * 2, 2);
        foreach (GameObject enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, player.transform.position) <= ability2_range)
            {
                enemy.GetComponent<EnemyMovement>().slow(ability2_slow_magnitude, ability2_usetime);
            }
        }
    }
}
