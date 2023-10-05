using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    public void Enable();
    public void Disable();

    public void boostFirerate(float magnitude, float usetime);

    public int GetUpgradeCost();

    public TowerType getTowerType();

    public void Upgrade(int op);

    public int GetStatus(); //returns current level of tower NOTE: 4a and 4b are both 4 as this is for upgrading and both can not be upgraded

    public int GetPrice();

    public void updatePrice(int increase);

    public TowerStats GetStats();

    public Material getMaterial();

    public bool getEnabled();

    public string getPassiveChosen();

    public void setPassiveChosen(string setter);

    public GameObject GetGameObject();

    public void EnableRange();

    public void DisableRange();

    public void Slow(float magnitude, float usetime);
}

public enum TowerType
{
    Archer, Mage, Mortar, Crystal
}
