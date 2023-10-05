using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int resourceTotal;
    public int maxResource = 99999;

    public TextMeshProUGUI resourceText;
    public TextMeshProUGUI moneyBoostText;

    AudioSource MoneyGained;

    int moneyBoost = 1;

    public void setMoneyBoost(int setter, float usetime) 
    {
        Debug.Log("money boost!");
        int original = moneyBoost;
        moneyBoost = setter;
        StartCoroutine(waitUseTime(usetime, original));
    }

    IEnumerator waitUseTime(float usetime, int original)
    {
        Debug.Log("active");
        moneyBoostText.gameObject.SetActive(true);
        yield return new WaitForSeconds(usetime);
        moneyBoostText.gameObject.SetActive(false);
        moneyBoost = original;
        Debug.Log("deactive");
    }

    private void Start()
    {
        resourceText.SetText(resourceTotal.ToString("d5"));
        MoneyGained = GameObject.Find("MoneyGained").GetComponent<AudioSource>();
    }

    public int spendResource(int amount)
    {
        int lost = 0;
        if (resourceTotal - amount < 0)
        {
            lost = resourceTotal;
            resourceTotal = 0;
        }
        else
        {
            lost = amount;
            resourceTotal -= amount;
        }
        resourceText.SetText(resourceTotal.ToString("d5"));
        return lost;
    }

    public void addResource(int amount, bool isCrystalTower = false)
    {
        if (isCrystalTower)
        {
            MoneyGained.Play();
        }
        
        if (resourceTotal + (amount*moneyBoost) > maxResource)
        {
            resourceTotal = maxResource;
        }
        else
        {
            resourceTotal += (amount*moneyBoost);
        }
        resourceText.SetText(resourceTotal.ToString("d5"));
    }

    public bool hasEnoughResource(int amount)
    {
        return resourceTotal >= amount;
    }
}
