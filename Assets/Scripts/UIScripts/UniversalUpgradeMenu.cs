using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UniversalUpgradeMenu : MonoBehaviour
{
    public ITower curTower;

    public TextMeshProUGUI CurrentTower;
    public TextMeshProUGUI ButtonA; // upgrade A path
    public TextMeshProUGUI ButtonB; // upgrade B path
    public TextMeshProUGUI ButtonC; // single path button
    public TextMeshProUGUI SellButton;

    public AudioSource upgradeSound;
    public AudioSource upgradeErrorSound;
    public GameObject UpgradeEffect;

    private ResourceManager resourceManager;
    private float sellFactor = 0.70f;

    public void Start()
    {
        resourceManager = GameObject.FindGameObjectWithTag("ResourceManager").GetComponent<ResourceManager>();
    }


    // add attr to stats > single vs double paths
    // add attr to hold path names if double path
    public void UpdateText(ITower tower)
    {
        curTower = tower;
        TowerStats stats = tower.GetStats();

        setCurrentTower(stats.Type, stats.Level);

        if (stats.Level == 4) { setPathCompleteMenu(stats); }
        else if (stats.twoPaths) { setDoublePathMenu(stats); }
        else { setSinglePathMenu(stats); }
    }

    public void Upgrade(int choice)
    {
        if (GameManager.instance.tutorialMode && curTower.getTowerType() == TowerType.Mortar && BuildManager.instance.usedMortarTarget) // allow mortar upgrades in tutorial after target used
        {
            BuildManager.instance.menuChange = true;
            BuildManager.instance.upgradedMortar++;
                PerformUpgrade(choice);
        }
        else if (!GameManager.instance.blockUpgrades)
        {
            BuildManager.instance.menuChange = true;
            PerformUpgrade(choice);
        }
    }

    void PerformUpgrade(int choice)
    {
        TowerStats stats = curTower.GetStats();
        if (stats.Level == 4) { return; }
        int upgradeCost = 0;

        if (choice == 0) { upgradeCost = stats.UpgradeCostA; }             // ButtonA or ButtonC
        else if (choice == 1) { upgradeCost = stats.UpgradeCostB; }        // ButtonB

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

    public void Sell()
    {
        if ((GameManager.instance.tutorialMode && !BuildManager.instance.blockSell) || !GameManager.instance.tutorialMode)
        {
            BuildManager.instance.soldTower = true;
            TowerStats stats = curTower.GetStats();
            BuildManager.instance.SellTower();
            resourceManager.addResource((int)(stats.Price * sellFactor) + 1);
        }

    }

    void setSinglePathMenu(TowerStats stats)
    {
        ButtonA.transform.parent.gameObject.SetActive(false);
        ButtonB.transform.parent.gameObject.SetActive(false);
        ButtonC.transform.parent.gameObject.SetActive(true);

        ButtonC.transform.parent.GetComponent<Button>().interactable = true;

        SellButton.text = string.Format("Sell for : ${0}", (int)(stats.Price * sellFactor) + 1);

        ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
        TowerStats newstat = UpgradeA.GetStats();

        float DPS = stats.FireRate * stats.ProjDamage;
        float newDPS = newstat.FireRate * newstat.ProjDamage;

        DPS = Mathf.Round(DPS * 100.0f) * 0.01f;
        newDPS = Mathf.Round(newDPS * 100.0f) * 0.01f;


        string tempButtonCText = "";

        // Upgrade or Ability Text
        if (stats.Type == TowerType.Crystal && stats.Level == 3)
            tempButtonCText += "<color=red><b>Passive</b></color>\n";
        else
            tempButtonCText += "<color=white><b>Upgrade</b></color>\n";


        // Range Text
        if (stats.Type == TowerType.Crystal && stats.Level == 3)
            tempButtonCText += "Range: <color=#5CC637>n/a</color>\n";
        else
        {
            if (stats.Range == newstat.Range)
                tempButtonCText += string.Format("Range: {0}\n", stats.Range);
            else
                tempButtonCText += string.Format("Range: {0} > <color=#5CC637>{1}</color>\n", stats.Range, newstat.Range);
        }


        // Rate and Damage Text
        if (stats.Type == TowerType.Crystal)
        {
            if (stats.FireRate == newstat.FireRate)
                tempButtonCText += string.Format("Rate: {0}\n", stats.FireRate);
            else
                tempButtonCText += string.Format("Rate: {0} > <color=#5CC637>{1}</color>\n", stats.FireRate, newstat.FireRate);

            if (stats.ProjDamage == newstat.ProjDamage)
                tempButtonCText += string.Format("Drop Value: {0}\n", stats.ProjDamage);
            else
                tempButtonCText += string.Format("Drop Value: {0} > <color=#5CC637>{1}</color>\n", stats.ProjDamage, newstat.ProjDamage);
        }
        else
        {
            if (DPS == newDPS)
                tempButtonCText += string.Format("DPS: {0}\n", DPS);
            else
                tempButtonCText += string.Format("DPS: {0} > <color=#5CC637>{1}</color>\n", DPS, newDPS);
        }


        // Drop Timer or Splash Radius or Num Targets
        if (stats.Type == TowerType.Crystal)
        {
            if (stats.Level == 3)
                tempButtonCText += string.Format("Drop Timer: <color=#5CC637>n/a</color>\n");
            else
            {
                if (stats.duration == newstat.duration)
                    tempButtonCText += string.Format("Drop Timer: {0}\n", stats.duration);
                else
                    tempButtonCText += string.Format("Drop Timer: {0} > <color=#5CC637>{1}</color>\n", stats.duration, newstat.duration);
            }
        }
        else if (stats.Type == TowerType.Mortar)
        {
            if (stats.duration == newstat.duration)
                tempButtonCText += string.Format("Dmg Radius: {0}\n", stats.duration);
            else
                tempButtonCText += string.Format("Dmg Radius: {0} > <color=#5CC637>{1}</color>\n", stats.duration, newstat.duration);
        }
        else
        {
            if (stats.NumTargets == newstat.NumTargets)
                tempButtonCText += string.Format("Targets: {0}\n", stats.NumTargets);
            else
                tempButtonCText += string.Format("Targets: {0} > <color=#5CC637>{1}</color>\n", stats.NumTargets, newstat.NumTargets);
        }


        // Upgrade Cost Text
        tempButtonCText += string.Format("Buy For: <color=white>{0}</color>\n", stats.UpgradeCostA);


        // SET BUTTON TEXT
        ButtonC.text = tempButtonCText;



        //// Crystal Factories should not have "damage"
        //if (stats.Type == TowerType.Crystal)
        //{
        //    // Level 3 > 4, show that it becomes passive
        //    if (stats.Level == 3)
        //    {
        //        ButtonC.text = string.Format(
        //            "<color=red><b>Passive</b></color>\n" +
        //            "Range: {0} > <color=#5CC637>n/a</color>\n" +
        //            "Rate: {2} > <color=#5CC637>{3}</color>\n" +
        //            "Drop Value: {4} > <color=#5CC637>{5}</color>\n" +
        //            "Drop Timer: {6} > <color=#5CC637>n/a</color>\n" +
        //            "Buy For: <color=white>{8}</color>", stats.Range, newstat.Range, stats.FireRate, newstat.FireRate, stats.ProjDamage, newstat.ProjDamage, stats.duration, newstat.duration, stats.UpgradeCostA);
        //    }

        //    else
        //    {
        //        ButtonC.text = string.Format(
        //            "<color=white><b>Upgrade</b></color>\n" +
        //            "Range: {0} > <color=#5CC637>{1}</color>\n" +
        //            "Rate: {2} > <color=#5CC637>{3}</color>\n" +
        //            "Drop Value: {4} > <color=#5CC637>{5}</color>\n" +
        //            "Drop Timer: {6} > <color=#5CC637>{7}</color>\n" +
        //            "Buy For: <color=white>{8}</color>", stats.Range, newstat.Range, stats.FireRate, newstat.FireRate, stats.ProjDamage, newstat.ProjDamage, stats.duration, newstat.duration, stats.UpgradeCostA);
        //    }
        //}

        //// Mortar Towers (splash radius hardcoded for now)
        //else if (stats.Type == TowerType.Mortar)
        //{
        //    ButtonC.text = string.Format(
        //        "<color=white><b>Upgrade</b></color>\n" +
        //        "Range: {0} > <color=#5CC637>{1}</color>\n" +
        //        "DPS: {2} > <color=#5CC637>{3}</color>\n" +
        //        "Splash Radius: 5\n\n" +
        //        "Buy For: <color=white>{4}</color>", stats.Range, newstat.Range, DPS, newDPS, stats.UpgradeCostA);
        //}

        //// Archer and Mage Towers
        //else
        //{
        //    ButtonC.text = string.Format(
        //        "<color=white><b>Upgrade</b></color>\n" +
        //        "Range: {0} > <color=#5CC637>{1}</color>\n" +
        //        "DPS: {2} > <color=#5CC637>{3}</color>\n" + 
        //        "Targets: {4} > <color=#5CC637>{5}</color>\n\n" +
        //        "Buy For: <color=white>{6}</color>", stats.Range, newstat.Range, DPS, newDPS, stats.NumTargets, newstat.NumTargets, stats.UpgradeCostA);
        //}
    }

    void setDoublePathMenu(TowerStats stats)
    {
        ButtonA.transform.parent.gameObject.SetActive(true);
        ButtonB.transform.parent.gameObject.SetActive(true);
        ButtonC.transform.parent.gameObject.SetActive(false);

        SellButton.text = string.Format("Sell for : ${0}", (int)(stats.Price * sellFactor) + 1);

        string abilityA = stats.pathA_name;
        string abilityB = stats.pathB_name;

        ITower UpgradeA = stats.UpgradeA.GetComponent<ITower>();
        TowerStats newstatA = UpgradeA.GetStats();

        float newDPS_A = newstatA.FireRate * newstatA.ProjDamage;
        newDPS_A = Mathf.Round(newDPS_A * 100.0f) * 0.01f;

        float DPS = stats.FireRate * stats.ProjDamage;
        DPS = Mathf.Round(DPS * 100.0f) * 0.01f;



        string tempButtonAText = "";
        tempButtonAText += string.Format("<color=red> {0} </color>\n", abilityA);

        // Range
        if (stats.Range == newstatA.Range)
            tempButtonAText += string.Format(" Range: {0}\n", stats.Range);
        else
            tempButtonAText += string.Format(" Range: {0} > <color=green>{1}</color>\n", stats.Range, newstatA.Range);


        // DPS
        if (DPS == newDPS_A)
            tempButtonAText += string.Format(" DPS: {0}\n", DPS);
        else
            tempButtonAText += string.Format(" DPS: {0} > <color=green>{1}</color>\n", DPS, newDPS_A);


        // Splash or Targets
        if (stats.Type == TowerType.Mortar)
        {
            if (stats.duration == newstatA.duration)
                tempButtonAText += string.Format(" Dmg Radius: {0}\n\n", stats.duration);
            else
                tempButtonAText += string.Format(" Dmg Radius: {0} > <color=green>{1}</color>\n\n", stats.duration, newstatA.duration);
        }
        else
        {
            if (stats.NumTargets == newstatA.NumTargets)
                tempButtonAText += string.Format(" Targets: {0}\n\n", stats.NumTargets);
            else
                tempButtonAText += string.Format(" Targets: {0} > <color=green>{1}</color>\n\n", stats.NumTargets, newstatA.NumTargets);
        }

        tempButtonAText += string.Format(" Buy For: <color=white>{0}</color>", stats.UpgradeCostA);
        ButtonA.text = tempButtonAText;


        //if (stats.Type == TowerType.Mortar)
        //    ButtonA.text = string.Format("<color=red> {0} </color>\n Range: {1}\n DPS: {2}\n Splash Radius: 10\n\n Buy For: <color=white>{3}</color>", abilityA, newstatA.Range, newDPS_A, stats.UpgradeCostA);
        //else
        //    ButtonA.text = string.Format("<color=red> {0} </color>\n Range: {1}\n DPS: {2}\n Targets: {3}\n\n Buy For: <color=white>{4}</color>", abilityA, newstatA.Range, newDPS_A, newstatA.NumTargets, stats.UpgradeCostA);





        ITower UpgradeB = stats.UpgradeB.GetComponent<ITower>();
        TowerStats newstatB = UpgradeB.GetStats();

        float newDPS_B = newstatB.FireRate * newstatB.ProjDamage;
        newDPS_B = Mathf.Round(newDPS_B * 100.0f) * 0.01f;


        string tempButtonBText = "";
        tempButtonBText += string.Format("<color=red> {0} </color>\n", abilityB);

        // Range
        if (stats.Range == newstatB.Range)
            tempButtonBText += string.Format(" Range: {0}\n", stats.Range);
        else
            tempButtonBText += string.Format(" Range: {0} > <color=green>{1}</color>\n", stats.Range, newstatB.Range);


        // DPS
        if (DPS == newDPS_B)
            tempButtonBText += string.Format(" DPS: {0}\n", DPS);
        else
            tempButtonBText += string.Format(" DPS: {0} > <color=green>{1}</color>\n", DPS, newDPS_B);


        // Splash or Targets
        if (stats.Type == TowerType.Mortar)
        {
            if (stats.duration == newstatB.duration)
                tempButtonBText += string.Format(" Dmg Radius: {0}\n\n", stats.duration);
            else
                tempButtonBText += string.Format(" Dmg Radius: {0} > <color=green>{1}</color>\n\n", stats.duration, newstatB.duration);
        }
        else
        {
            if (stats.NumTargets == newstatB.NumTargets)
                tempButtonBText += string.Format(" Targets: {0}\n\n", stats.NumTargets);
            else
                tempButtonBText += string.Format(" Targets: {0} > <color=green>{1}</color>\n\n", stats.NumTargets, newstatB.NumTargets);
        }

        tempButtonBText += string.Format(" Buy For: <color=white>{0}</color>", stats.UpgradeCostB);
        ButtonB.text = tempButtonBText;

        //if (stats.Type == TowerType.Mortar)
        //    ButtonB.text = string.Format("<color=red> {0} </color>\n Range: {1}\n DPS: {2}\n Splash Radius: 5\n\n Buy For: <color=white>{3}</color>", abilityB, newstatB.Range, newDPS_B, stats.UpgradeCostB);
        //else
        //    ButtonB.text = string.Format("<color=red> {0} </color>\n Range: {1}\n DPS: {2}\n Targets: {3}\n\n Buy For: <color=white>{4}</color>", abilityB, newstatB.Range, newDPS_B, newstatB.NumTargets, stats.UpgradeCostB);

    }

    void setPathCompleteMenu(TowerStats stats)
    {
        ButtonA.transform.parent.gameObject.SetActive(false);
        ButtonB.transform.parent.gameObject.SetActive(false);
        ButtonC.transform.parent.gameObject.SetActive(true);

        float DPS = stats.FireRate * stats.ProjDamage;
        DPS = Mathf.Round(DPS * 100.0f) * 0.01f;

        SellButton.text = string.Format("Sell for : ${0}", (int)(stats.Price * sellFactor) + 1);

        if (stats.Type == TowerType.Crystal)
            ButtonC.text = string.Format("<color=red>{0}</color>\n Range: n/a\n Rate: {2}\n Drop Value: {3}\n", curTower.getPassiveChosen(), stats.Range, stats.FireRate, stats.ProjDamage);
        else if (stats.Type == TowerType.Mortar)
            ButtonC.text = string.Format("<color=red>{0}</color>\n Range: {1}\n DPS: {2}\n Dmg Radius: {3}\n", curTower.getPassiveChosen(), stats.Range, DPS, stats.duration);
        else
            ButtonC.text = string.Format("<color=red>{0}</color>\n Range: {1}\n DPS: {2}\n Targets: {3}\n", curTower.getPassiveChosen(), stats.Range, DPS, stats.NumTargets);

        ButtonC.transform.parent.GetComponent<Button>().interactable = false;
    }


    void setCurrentTower(TowerType type, int level)
    {
        string ext = "Tower";
        if (type == TowerType.Crystal) { ext = "Factory"; }

        switch (level)
        {
            case 1:
                CurrentTower.text = string.Format("<color=#A419FF> {0} {2} {1} </color>", type, level, ext);
                break;
            case 2:
                CurrentTower.text = string.Format("<color=yellow> {0} {2} {1} </color>", type, level, ext);
                break;
            case 3:
                CurrentTower.text = string.Format("<color=#00DEFF> {0} {2} {1} </color>", type, level, ext);
                break;
            case 4:
                CurrentTower.text = string.Format("<color=#FF006F> {0} {2} {1} </color>", type, level, ext);
                break;
            default:
                break;

        }
    }
}



