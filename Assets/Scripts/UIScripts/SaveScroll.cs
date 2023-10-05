using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveScroll : MonoBehaviour
{
    [Tooltip("The name to store the data under.")]
    [SerializeField] protected string settingName;
    [Tooltip("The script to get the data from.")]
    [SerializeField] protected Slider reference;

    protected void Awake()
    {
        if (!reference || settingName.Length == 0) return;
        if (PlayerPrefs.GetFloat(settingName) == 0) { PlayerPrefs.SetFloat(settingName, 1); PlayerPrefs.Save(); }
        reference.value = PlayerPrefs.GetFloat(settingName);
        Debug.Log("setting " + reference.name + " to " + reference.value);
    }

    /// <summary> Saves the data from the corresponding component to the PlayerPrefs. </summary>
    public void Save()
    {
        if (!reference || settingName.Length == 0) return;
        PlayerPrefs.SetFloat(settingName, reference.value);
        PlayerPrefs.Save();
        GameManager.instance.updateVolume();
    }
}
