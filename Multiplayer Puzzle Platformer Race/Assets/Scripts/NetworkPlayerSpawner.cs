using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject spawnPlayerPrefab;

    public string playerPrefab = "blackPlayer";

    public Transform spawnPosWhite;
    public Transform spawnPosBlack;

    public Transform yourSpawnPos;

    public GameObject loadingOverlay;

    public GameObject connectingText;
    public GameObject waitingText;

    public TMP_Text countdown;

    public bool connected;

    public bool started;

    public PhotonView pv;

    public MenuManager menuManager;

    public AudioSource gameMusic;

    public AudioSource countdownSound;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        loadingOverlay.SetActive(true);
        connectingText.SetActive(true);
        waitingText.SetActive(false);

        started = false;

        countdown.text = "";

        connected = false;
    }
    private void Update()
    {
        if (PhotonNetwork.PlayerList.Length == 2 && spawnPlayerPrefab != null && !started && PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            pv.RPC("StartGame", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        started = true;
        loadingOverlay.SetActive(false);

        Debug.Log("Success");

        StartCoroutine("startDelay");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        connectingText.SetActive(false);
        waitingText.SetActive(true);
        StartCoroutine("spawnDelay");
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnPlayerPrefab);
    }

    public IEnumerator spawnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        spawnPlayerPrefab = PhotonNetwork.Instantiate(playerPrefab, yourSpawnPos.position, yourSpawnPos.rotation);
        spawnPlayerPrefab.gameObject.name = "Local Network Player";

        if (playerPrefab == "blackPlayer")
        {
            spawnPlayerPrefab.GetComponent<PlayerController>().isBlack = true;
        }

        else if (playerPrefab == "whitePlayer")
        {
            spawnPlayerPrefab.GetComponent<PlayerController>().isBlack = false;
        }

        connected = true;
    }

    public IEnumerator startDelay()
    {
        menuManager.LoadGame();
        yield return new WaitForSeconds(0.5f);
        countdownSound.Play();
        countdown.text = "3";
        yield return new WaitForSeconds(1f);
        countdown.text = "2";
        yield return new WaitForSeconds(1f);
        countdown.text = "1";
        yield return new WaitForSeconds(1f);
        countdown.text = "GO";
        spawnPlayerPrefab.GetComponent<PlayerController>().inGame = true;
        StartCoroutine(AudioFadeOut.FadeIn(gameMusic, 1.25f));
        yield return new WaitForSeconds(0.5f);
        countdown.text = "";
    }
}
