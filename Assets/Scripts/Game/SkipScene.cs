using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipScene : MonoBehaviour
{
    [SerializeField] string nextScene;

    public void gotoNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
