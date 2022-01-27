using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public NetworkPlayerSpawner playerSpawner;

    public bool yourServer = false;

    public string username = "";

    public TMPro.TMP_Text displayText;

    // Start is called before the first frame update
    void Start()
    {
        yourServer = false;
        username = PlayerPrefs.GetString("username");

        displayText.text = "... vs " + username;

        ConnectToServer();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");

        base.OnConnectedToMaster();

        PhotonNetwork.JoinRandomRoom();

        PhotonNetwork.LocalPlayer.NickName = username;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No Rooms Found, Creating a New Room");

        RoomOptions roomOptions = new RoomOptions();

        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        string randomRoomName = "" + Random.Range(0, 1000);

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions, TypedLobby.Default);

        yourServer = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a Room");

        if (yourServer)
        {
            playerSpawner.yourSpawnPos = playerSpawner.spawnPosBlack;
            playerSpawner.playerPrefab = "blackPlayer";
        }

        else if (!yourServer)
        {
            playerSpawner.yourSpawnPos = playerSpawner.spawnPosWhite;
            playerSpawner.playerPrefab = "whitePlayer";
        }

        base.OnJoinedRoom();
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to Connect");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room");

        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Disconnected from room");
        base.OnLeftRoom();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
