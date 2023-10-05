using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI HP;

    public GameObject background;
    public List<GameObject> borders;
    private int borderIdx = -1;

    public Image heartImg;
    public List<Sprite> heartSprites;
    private int spriteIdx = -1;
    private Vector3 spriteScale2 = new Vector3(2.5f, 2.5f, 2.5f);

    private float expandMultiplier = 0.50f;

    public void SetMaxHealth(float newMax)
    {
        float delta = newMax - slider.maxValue;

        //Debug.Log("delta: " + delta + " max: " + slider.maxValue + " newMax: " + newMax + " borderIdx: " + borderIdx);
        slider.maxValue = newMax;

        if (delta > 0)
        {
            delta *= expandMultiplier;
            background.GetComponent<RectTransform>().sizeDelta = new Vector2(background.GetComponent<RectTransform>().rect.width + delta, background.GetComponent<RectTransform>().rect.height);
            background.GetComponent<RectTransform>().localPosition += new Vector3((delta / 2), 0, 0);
        }

        //if (borderIdx > -1 && borderIdx <= borders.Count && borders[borderIdx])
        //    borders[borderIdx].SetActive(true);
        //if (borderIdx > 0 && borders[borderIdx - 1])
        //    borders[borderIdx - 1].SetActive(false);
        //borderIdx++;

        if (spriteIdx > -1 && spriteIdx <= heartSprites.Count-1 && heartSprites[spriteIdx])
            heartImg.sprite = heartSprites[spriteIdx];
        if (spriteIdx >= 2)
            heartImg.gameObject.transform.localScale = spriteScale2;
        spriteIdx++;

        SetHealth(slider.value);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        HP.text = (Mathf.Round(health * 100.0f) * 0.01f).ToString() + "/" + slider.maxValue.ToString();
    }
}
