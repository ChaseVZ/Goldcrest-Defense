using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats
{
    public TowerType Type;
    public int Level;
    public float Range;
    public float FireRate;
    public float ProjDamage;
    public int Price;
    public int UpgradeCostA;
    public int UpgradeCostB;
    public GameObject UpgradeA;
    public GameObject UpgradeB;
    public int NumTargets;
    public string passiveChosen;
    public bool twoPaths; // if True, two upgrade paths are available
    public string pathA_name;
    public string pathB_name;

    public float duration; // used for Crystal Factory timer and mortar splash dmg

    public TowerStats(TowerType _Type, int _Level, float _Range, float _FireRate, float _ProjDamage, int _Price, int _UpgradeCostA, int _UpgradeCostB, GameObject _UpgradeA, GameObject _UpgradeB, int _NumTarget, bool _twoPaths, string _aName, string _bName)
    {
        Type = _Type;
        Level = _Level;
        Range = _Range;
        FireRate = _FireRate;
        ProjDamage = _ProjDamage;
        Price = _Price;
        UpgradeCostA = _UpgradeCostA;
        UpgradeCostB = _UpgradeCostB;
        UpgradeA = _UpgradeA;
        UpgradeB = _UpgradeB;
        NumTargets = _NumTarget;
        twoPaths = _twoPaths;
        pathA_name = _aName;
        pathB_name = _bName;
        duration = 0;
    }

    // For ones that have a duration
    public TowerStats(TowerType _Type, int _Level, float _Range, float _FireRate, float _ProjDamage, int _Price, int _UpgradeCostA, int _UpgradeCostB, GameObject _UpgradeA, GameObject _UpgradeB, int _NumTarget, bool _twoPaths, string _aName, string _bName, float _duration)
    {
        Type = _Type;
        Level = _Level;
        Range = _Range;
        FireRate = _FireRate;
        ProjDamage = _ProjDamage;
        Price = _Price;
        UpgradeCostA = _UpgradeCostA;
        UpgradeCostB = _UpgradeCostB;
        UpgradeA = _UpgradeA;
        UpgradeB = _UpgradeB;
        NumTargets = _NumTarget;
        twoPaths = _twoPaths;
        pathA_name = _aName;
        pathB_name = _bName;
        duration = _duration;
    }
}
