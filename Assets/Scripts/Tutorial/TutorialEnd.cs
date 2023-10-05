using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.tutorialMode = false;
            BuildManager.instance.unsetTutorialMode();
            SceneManager.LoadScene("Main_2");
        }
    }
}