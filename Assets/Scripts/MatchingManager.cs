using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour {
    [SerializeField]
    private Text messageText;
    [SerializeField]
    private Text statusText;

    private string ProgressDot {
        get {
            string str = "";
            int numberOfDots = Mathf.FloorToInt(Time.timeSinceLevelLoad * 3f % 4);

            for (int i = 0; i < numberOfDots; ++i) {
                str += " .";
            }

            return str;
        }
    }

    private void Awake() {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated) {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        // generate a name for this player, if none is assigned yet
        if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {
            PhotonNetwork.playerName = @"プレイヤー";
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }

    private void Update() {
        if (PhotonNetwork.inRoom && PhotonNetwork.countOfPlayersInRooms > 1) {
            messageText.text = $"マッチングしました。";
            statusText.text = $"ユーザー数: {PhotonNetwork.countOfPlayers}, 部屋数: {PhotonNetwork.countOfRooms}";
        } else {
            messageText.text = $"マッチング中です{ ProgressDot }";
            statusText.text = $"ユーザー数: {PhotonNetwork.countOfPlayers}, 部屋数: {PhotonNetwork.countOfRooms}";
        }
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
    }

    public void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnPhotonCreateRoomFailed() {
        Debug.Log("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonJoinRoomFailed(object[] cause) {
        Debug.Log("OnPhotonJoinRoomFailed");
    }

    public void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        var roomOptions = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }

    public void OnCreatedRoom() {
        Debug.Log("OnCreatedRoom");
    }

    public void OnDisconnectedFromPhoton() {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters) {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    public void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster()");
        PhotonNetwork.JoinLobby();
    }
}
