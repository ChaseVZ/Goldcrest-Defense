using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth;
    public float maximumUpgradableHealth = 200;
    //camera variable......

    public GameOverScreen gameOverScreen;

    public HealthBar healthBar;
    public GameObject redFlash;

    AudioSource hitmarker;

    void Start()
    {
        currentHealth = maxHealth;
        redFlash.SetActive(false);
        healthBar.SetMaxHealth(maxHealth);
        hitmarker = GetComponent<AudioSource>();
    }
    
    void GameOver()
    {
        // TODO: Load up a game over scene or a start menu... 
        GameManager.instance.healthDepleted();
        gameOverScreen.ShowScreen();
    }

    // TODO: Add camerashake/red screen shake on damage taken...
    public void TakeDamage(float damage)
    {
        hitmarker.Play();
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        //TODO: what do I want to do on damage taken ?
        //StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    public void AddHealth(float health)
    {
        if (currentHealth + health > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += health;
        }
        healthBar.SetHealth(currentHealth);
    }

    IEnumerator DamageFlash()
    {
        //Flash red when taking damage
        redFlash.SetActive(true);
        yield return new WaitForSeconds(.2f);
        redFlash.SetActive(false);
    }

    public void setMaxHealth(float newMax)
    {
        this.maxHealth = newMax;
    }

    public bool upgradeHealth(float amount)
    {
        this.maxHealth += amount;
        healthBar.SetMaxHealth(maxHealth);

        if (maxHealth >= maximumUpgradableHealth)
        {
            maxHealth = maximumUpgradableHealth;
            healthBar.SetMaxHealth(maxHealth);
            return true;
        }
        return false;
    }

}
