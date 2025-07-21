using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Trader : MonoBehaviour
{
    public TownHealth townHealth;
    private ResourceManager resourceManager;

    public Button healthButton;
    public float upgradeHealthAmount = 10f;
    public float healthUpgradeIncreaseRate = 5f;
    private float healthCost = 1200f;

    public Button mvmtSpeedButton;
    public float upgradeMovementSpeedAmount = 5f;
    private float mvmtCost = 800f;

    public Button swordButton;
    public float upgradeDmgAmount = 1f;
    public float dmgCost = 1600f;
    public bool hasWeapon = false;

    public PlayerController player;
    public GameObject playerAvatar;

    public List<Material> playerMaterials;
    private int playerMaterialsIdx = 0;

    public AudioSource upgradeSound;
    public AudioSource upgradeErrorSound;

    private void Awake()
    {
        // Find ResourceManager if not assigned
        if (resourceManager == null)
        {
            resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
        }
        
        healthButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Max Health: <color=white>{0}</color>", healthCost.ToString());
        mvmtSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Movement Speed: <color=white>{0}</color>", mvmtCost.ToString());
        swordButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Unlock Sword: <color=white>{0}</color>", dmgCost.ToString());
    }

    public void upgradeHealth()
    {
        // Check if player has enough resources
        if (!resourceManager.hasEnoughResource((int)healthCost))
        {
            upgradeErrorSound.Play();
            return; // Not enough resources, don't proceed
        }

        // Spend the resources
        resourceManager.spendResource((int)healthCost);
        upgradeSound.Play();

        if (townHealth.upgradeHealth(upgradeHealthAmount))
        {
            healthButton.interactable = false;
            healthButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
        else
        {
            healthCost *= 1.5f;
            healthButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Max Health: <color=white>{0}</color>", healthCost.ToString());
        }

        upgradeHealthAmount += healthUpgradeIncreaseRate;
    }

    public void upgradeMovementSpeed()
    {
        // Check if player has enough resources
        if (!resourceManager.hasEnoughResource((int)mvmtCost))
        {
            upgradeErrorSound.Play();
            return; // Not enough resources, don't proceed
        }

        // Spend the resources
        resourceManager.spendResource((int)mvmtCost);
        upgradeSound.Play();

        if (playerMaterialsIdx < playerMaterials.Count)
            playerAvatar.GetComponent<Renderer>().material = playerMaterials[playerMaterialsIdx];
        playerMaterialsIdx++;

        if (player.upgradeMovementSpeed(upgradeMovementSpeedAmount))
        {
            mvmtSpeedButton.interactable = false;
            mvmtSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text = "MAX";
        }
        else
        {
            mvmtCost *= 1.5f;
            mvmtSpeedButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Movement Speed: <color=white>{0}</color>", mvmtCost.ToString());
        }
    }

    public void upgradeWeapon()
    {
        // Check if player has enough resources
        if (!resourceManager.hasEnoughResource((int)dmgCost))
        {
            upgradeErrorSound.Play();
            return; // Not enough resources, don't proceed
        }

        // Spend the resources
        resourceManager.spendResource((int)dmgCost);
        upgradeSound.Play();

        // Apply the weapon upgrade logic here
        hasWeapon = true;
        
        dmgCost *= 1.5f;
        swordButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Sword Damage: <color=white>{0}</color>", dmgCost.ToString());
    }
}
