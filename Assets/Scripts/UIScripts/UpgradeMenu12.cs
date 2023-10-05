using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeMenu12 : MonoBehaviour
{
    public ITower curTower;

    public TextMeshProUGUI CurrentTower;
    public TextMeshProUGUI TowerStats;
    public TextMeshProUGUI UpgradeStats;
    public TextMeshProUGUI UpgradeCost;

    public AudioSource upgradeSound;
    public GameObject UpgradeEffect;
    public AudioSource upgradeErrorSound;
    private ResourceManager resourceManager;

    public void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }
    public void UpdateText(ITower tower)
    {
        curTower = tower;
        TowerStats stats = tower.GetStats();
        if (stats.Type == TowerType.Crystal)
        {
            if (stats.Level == 3)
            {
                CurrentTower.text = stats.Type + " " + stats.Level;
                TowerStats.text = "Tower Stats:\n\n" + "Range: " + stats.Range + "\nFire Rate: " + stats.FireRate +
                    "\nMoney: " + stats.ProjDamage;
                UpgradeCost.text = "Upgrade: " + stats.UpgradeCostA;
                ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
                TowerStats newstat = UpgradeA.GetStats();
                UpgradeStats.text = "Ability: Passive" + "\nFire Rate: " + stats.FireRate + " > <color=green>" + newstat.FireRate +
                    "</color>\nMoney: " + stats.ProjDamage + "> <color=green>"+ newstat.ProjDamage + "</color>";
            }
            else
            {
                CurrentTower.text = stats.Type + " " + stats.Level;
                TowerStats.text = "Tower Stats:\n" + "Range: " + stats.Range + "\nFire Rate: " + stats.FireRate +
                    "\nMoney: " + stats.ProjDamage;
                UpgradeCost.text = "Upgrade: " + stats.UpgradeCostA;
                ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
                TowerStats newstat = UpgradeA.GetStats();
                UpgradeStats.text = "Range: " + newstat.Range + "\nFire Rate: " + stats.FireRate + " > <color=green>" + newstat.FireRate +
                    "</color>\nMoney: " + stats.ProjDamage + "> <color=green>" + newstat.ProjDamage + "</color>";
            }
        }
        else
        {
            if (stats.Level == 1) { CurrentTower.text = "<color=#A419FF>" + stats.Type + " Level " + stats.Level + "</color>"; }
            if (stats.Level == 2) { CurrentTower.text = "<color=yellow>" + stats.Type + " Level " + stats.Level + "</color>"; }
           

            TowerStats.text = "Tower Stats:\n" + "Range: " + stats.Range + "\nFire Rate: " + stats.FireRate +
                "\nDamage: " + stats.ProjDamage + "\nTargets: " + stats.NumTargets;
            UpgradeCost.text = "Upgrade: " + stats.UpgradeCostA;
            ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
            TowerStats newstat = UpgradeA.GetStats();
            UpgradeStats.text = "Range: " + stats.Range + " > <color=green>" + newstat.Range + "</color>\nFire Rate: " + stats.FireRate + " > <color=green>" + newstat.FireRate +
                "</color>\nDamage: " + stats.ProjDamage + " > <color=green>" + newstat.ProjDamage + "</color>\nTargets: " + stats.NumTargets + " > <color=green>" + newstat.NumTargets + "</color>";
        }

    }

    public void Upgrade()
    {
        if (resourceManager.hasEnoughResource(curTower.GetUpgradeCost()))
        {
            resourceManager.spendResource(curTower.GetUpgradeCost());
            Quaternion qt = curTower.GetGameObject().transform.rotation;
            qt.Set(0, 0, 0, 0);
            Vector3 pos = curTower.GetGameObject().transform.position;
            GameObject effect = (GameObject)Instantiate(UpgradeEffect, pos, qt);
            effect.transform.rotation *= Quaternion.Euler(-90, 0, 0);
            effect.GetComponent<ParticleSystem>().Play();
            Destroy(effect, 3f);
            upgradeSound.Play();
            curTower.Upgrade(0);
        }
        else
        {
            upgradeErrorSound.Play();
        }
    }
}
