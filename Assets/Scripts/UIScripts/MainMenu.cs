using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button PLAY;
    [SerializeField] GameObject playImage;
    [SerializeField] AudioSource backgroundMusic;

    public float musicVolume;

    public void Start()
    {
        backgroundMusic.volume = 0f;
        backgroundMusic.Play();
        StartCoroutine(StartFade(backgroundMusic, 7f, musicVolume)); //* GameManager.instance.OverallAudioScalar * GameManager.instance.MusicAudioScalar
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = 0f;
        while (currentTime < duration)
        {
            Debug.Log(audioSource.volume);
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        playImage.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter.name);
        if (eventData.pointerEnter == PLAY)
        {
            playImage.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerEnter == PLAY)
        {
            playImage.SetActive(false);
        }
    }

}
