using System;
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

    private bool startedMatching = false;

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
        PhotonNetwork.automaticallySyncScene = true;

        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated) {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {
            PhotonNetwork.playerName = @"Guest";
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    private void Update() {
        if (!startedMatching) {
            return;
        }

        if (PhotonNetwork.inRoom && PhotonNetwork.room.PlayerCount > 1) {
            messageText.text = @"マッチングしました!";
        } else {
            messageText.text = @"マッチング中です" + ProgressDot;
        }
        if (PhotonNetwork.inRoom) {
            if (PhotonNetwork.room.PlayerCount > 1) {
                statusText.text = @"マッチング相手: " + PhotonNetwork.otherPlayers[0].NickName;
            } else {
                statusText.text = @"あなた: " + PhotonNetwork.playerName;
            }
        } else {
            statusText.text = @"接続中";
        }
    }

    private IEnumerator requestJoinRandomRoom() {
        while (!PhotonNetwork.connectedAndReady) {
            yield return new WaitForEndOfFrame();
        };
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartMatching() {
        startedMatching = true;
        StartCoroutine(requestJoinRandomRoom());
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        Debug.Log($"A player(id: { player.ID }) has left room.");
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log($"Joined player(id: { player.ID }) in this room.");
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
    }

    public void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
    }

    public void OnPhotonCreateRoomFailed() {
        Debug.Log("OnPhotonCreateRoomFailed");
    }

    public void OnPhotonJoinRoomFailed(object[] cause) {
        Debug.Log("OnPhotonJoinRoomFailed");
    }

    public void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom("", new RoomOptions{ MaxPlayers = 2 }, null);
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
