using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialBoxTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private bool doOnce = true;

    private TutorialManager tutorialManager;
    void Start()
    {
        tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && doOnce)
        {
            tutorialManager.incrementProg();
            doOnce = false;
        }
    }
}
