using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ArcherText;
    [SerializeField] TextMeshProUGUI MageText;
    [SerializeField] TextMeshProUGUI MortarText;
    [SerializeField] TextMeshProUGUI FactoryText;

    [SerializeField] GameObject archerPrefab;
    [SerializeField] GameObject magePrefab;
    [SerializeField] GameObject mortarPrefab;
    [SerializeField] GameObject factoryPrefab;

    void Start()
    {
        ArcherText.text = string.Format("Archer Tower  <color=#5CC637>${0}</color>", archerPrefab.GetComponent<ArcherTower>().GetPrice());
        MageText.text = string.Format("Mage Tower  <color=#5CC637>${0}</color>", magePrefab.GetComponent<MageTower>().GetPrice());
        MortarText.text = string.Format("Mortar Tower  <color=#5CC637>${0}</color>", mortarPrefab.GetComponent<MortarTower>().GetPrice());
        FactoryText.text = string.Format("Crystal Factory  <color=#5CC637>${0}</color>", factoryPrefab.GetComponent<CrystalFactoryTower>().GetPrice());
    }

}
