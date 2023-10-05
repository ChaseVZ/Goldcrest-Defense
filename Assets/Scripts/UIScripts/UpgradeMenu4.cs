using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeMenu4 : MonoBehaviour
{
    public ITower curTower;

    public TextMeshProUGUI CurrentTower;
    public TextMeshProUGUI TowerStats;


    public void UpdateText(ITower tower)
    {

        curTower = tower;
        TowerStats stats = tower.GetStats();
        if (stats.Type == TowerType.Crystal)
        {

            CurrentTower.text = stats.Type + " " + stats.Level;
            TowerStats.text = "<color=red>Passive\n</color>" + "Range: " + stats.Range + "\nFire Rate: " + stats.FireRate +
                "\nMoney: " + stats.ProjDamage;
        }
        else
        {
            CurrentTower.text = stats.Type + " Level " + stats.Level;
            TowerStats.text = "<color=red>" + curTower.getPassiveChosen() + "</color>\nRange: " + stats.Range + "\nFire Rate: " + stats.FireRate +
                "\nDamage: " + stats.ProjDamage + "\nTargets: " + stats.NumTargets;
        }
    }
}
