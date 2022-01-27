using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;


public class Trophy : MonoBehaviour
{

    public TMP_Text winText;
    public GameObject vsText;
    public PhotonView pv;
    public AudioSource normalMusic;
    public AudioSource winMusic;
    public GameObject confetti;

    public Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        winMusic.gameObject.SetActive(false);
        confetti.SetActive(false);
        winText.gameObject.SetActive(false);
        pv = GetComponent<PhotonView>();
        vsText = GameObject.FindGameObjectWithTag("DisplayName");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            pv.RPC("Win", RpcTarget.All, collision.gameObject.GetComponent<PlayerController>().username);
            Debug.Log("Won and ran RPC");
        }
    }

    IEnumerator WinSetup()
    {
        StartCoroutine(AudioFadeOut.FadeOut(normalMusic, 1.25f));

        yield return new WaitForSeconds(1.25f);

        StartCoroutine(AudioFadeOut.FadeIn(winMusic, 0.25f));
    }

    [PunRPC]
    public void Win(string _winnerName)
    {
        mainCam.transform.position = new Vector3(147.2f, 0, mainCam.transform.position.z);
        StartCoroutine("WinSetup");

        PlayerController[] gamePlayers;
        gamePlayers = FindObjectsOfType<PlayerController>();

        for (int i = 0; i < gamePlayers.Length; i++)
        {
            gamePlayers[i].GetComponent<PlayerController>().inGame = false;
            gamePlayers[i].GetComponent<Rigidbody2D>().gravityScale = 0;
            gamePlayers[i].GetComponent<BoxCollider2D>().enabled = false;
            gamePlayers[i].GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }

        Debug.Log("RPC ran");
        winMusic.gameObject.SetActive(true);
        vsText.SetActive(false);
        winText.gameObject.SetActive(true);
        winText.text = "Winner: " + _winnerName + "!";
        confetti.SetActive(true);

        StartCoroutine("EndGame");
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSeconds(30f);

        StartCoroutine(AudioFadeOut.FadeOut(winMusic, 3f));

        yield return new WaitForSeconds(2f);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
