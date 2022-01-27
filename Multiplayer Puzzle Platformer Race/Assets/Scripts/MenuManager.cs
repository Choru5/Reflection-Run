using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject fadeEffect;
    public GameObject fadeInEffect;
    public AudioSource music;
    public AudioSource menuMusic;

    private void Start()
    {
        StartCoroutine(AudioFadeOut.FadeIn(menuMusic, 1.25f));
        fadeEffect.SetActive(false);

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            fadeInEffect.SetActive(true);
            StartCoroutine("DestroyFaded");
        }

        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            fadeInEffect.SetActive(false);
        }
    }

    public void LoadGame()
    {
        StartCoroutine(AudioFadeOut.FadeOut(menuMusic, 1.25f));
        fadeInEffect.SetActive(true);
        StartCoroutine("DestroyFaded");
    }

    public void LoadScene(int _sceneID)
    {
        StartCoroutine("FadeThenLoad", _sceneID);
        StartCoroutine(AudioFadeOut.FadeOut(music, 1.25f));
    }

    public void ChangeName()
    {
        PlayerPrefs.SetString("username", nameInput.text);
    }

    public IEnumerator DestroyFaded()
    {
        yield return new WaitForSeconds(0.9f);
        fadeInEffect.SetActive(false);
    }

    public IEnumerator FadeThenLoad(int _ID)
    { 
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PhotonNetwork.Disconnect();
        }

        fadeEffect.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(_ID);
    }
}
