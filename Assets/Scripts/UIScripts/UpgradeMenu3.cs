using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeMenu3 : MonoBehaviour
{
    public ITower curTower;

    public TextMeshProUGUI CurrentTower;
    public TextMeshProUGUI TowerStats;
    public TextMeshProUGUI UpgradeStatsA;
    public TextMeshProUGUI UpgradeCostA;
    public TextMeshProUGUI UpgradeStatsB;
    public TextMeshProUGUI UpgradeCostB;

    public AudioSource upgradeSound;
    public AudioSource upgradeErrorSound;
    public GameObject UpgradeEffect;

    private ResourceManager resourceManager;

    public void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }


    public void UpdateText(ITower tower)
    {
        curTower = tower;
        TowerStats stats = tower.GetStats();
        CurrentTower.text = stats.Type + " Level " + stats.Level;
        //TowerStats.text = "Tower Stats:\n\n" + "Range: " + stats.Range + "\nFire Rate: " + stats.FireRate +
        //    "\nDamage: " + stats.ProjDamage + "\nTargets: " + stats.NumTargets;

        UpgradeCostA.text = "Upgrade: " + stats.UpgradeCostA;
        ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
        TowerStats newstat = UpgradeA.GetStats();
        string abilityA = "";
        string abilityB = "";
        if(stats.Type == TowerType.Archer)
        {
            abilityA = "Sniper";
            abilityB = "Dual Archer";
        }
        else if(stats.Type == TowerType.Mage)
        {
            abilityA = "Single Beam";
            abilityB = "Multi Beam";
        }
        else if(stats.Type == TowerType.Mortar)
        {
            abilityA = "Large Bomb";
            abilityB = "Triple Shot";
        }

        //UpgradeStatsA.text = "Upgrade Stats:\n" + "Ability: " + abilityA +"\nRange: " + newstat.Range + "\nFire Rate: " + newstat.FireRate +
        //    "\nDamage: " + newstat.ProjDamage + "\nTargets: " + newstat.NumTargets;

        UpgradeStatsA.text = "<color=red>" + abilityA + "</color>"+ "\nRange: " + newstat.Range + "\nFire Rate: " + newstat.FireRate +
            "\nDamage: " + newstat.ProjDamage + "\nTargets: " + newstat.NumTargets;

        UpgradeCostB.text = "Upgrade: " + stats.UpgradeCostB;
        ITower UpgradeB = stats.UpgradeB.GetComponent<ITower>();
        TowerStats newstatB = UpgradeB.GetStats();
        //UpgradeStatsB.text = "Upgrade Stats:\n" + "Ability: " + abilityB + "\nRange: " + newstatB.Range + "\nFire Rate: " + newstatB.FireRate +
        //    "\nDamage: " + newstatB.ProjDamage + "\nTargets: " + newstatB.NumTargets;

        UpgradeStatsB.text = "<color=red>" + abilityB + "</color>" + "\nRange: " + newstatB.Range + "\nFire Rate: " + newstatB.FireRate +
            "\nDamage: " + newstatB.ProjDamage + "\nTargets: " + newstatB.NumTargets;

    }

    public void Upgrade(int choice)
    {
        TowerStats stats = curTower.GetStats();
        int upgradeCost;
        if(choice == 1)
        {
            upgradeCost = stats.UpgradeCostB;
        }
        else
        {
            upgradeCost = stats.UpgradeCostA;
        }
        if (resourceManager.hasEnoughResource(upgradeCost))
        {
            resourceManager.spendResource(upgradeCost);
            Quaternion qt = curTower.GetGameObject().transform.rotation;
            qt.Set(0, 0, 0, 0);
            Vector3 pos = curTower.GetGameObject().transform.position;
            GameObject effect = (GameObject)Instantiate(UpgradeEffect, pos, qt);
            effect.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(effect, 3f);
            upgradeSound.Play();
            curTower.Upgrade(choice);
        }
        else
        {
            upgradeErrorSound.Play();
        }
    }
}
