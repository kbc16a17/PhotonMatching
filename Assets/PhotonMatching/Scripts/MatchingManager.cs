using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class MatchingManager : MonoBehaviour {
    [SerializeField]
    private GameObject matchingWindow;
    [SerializeField]
    private Text messageLabel;
    [SerializeField]
    private Text statusLabel;
    [SerializeField]
    private Button cancelButton;

    private bool finding = false;

    private void Awake() {
        PhotonNetwork.automaticallySyncScene = true;

        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated) {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        if (string.IsNullOrEmpty(PhotonNetwork.playerName)) {
            PhotonNetwork.playerName = @"Guest" + Random.Range(1, 9999);
        }

        // PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    private void Start() {
        matchingWindow.SetActive(true);
        matchingWindow.GetComponent<Animator>().Play(@"OpenWindow");
    }

    private IEnumerator requestJoinRandomRoom() {
        while (!PhotonNetwork.connectedAndReady) {
            yield return new WaitForEndOfFrame();
        };
        PhotonNetwork.JoinRandomRoom();
    }

    public void StartMatching() {
        finding = true;
        StartCoroutine(requestJoinRandomRoom());
    }

    public void CancelMatching() {
        finding = false;
        matchingWindow.GetComponent<Animator>().Play(@"CloseWindow");
        PhotonNetwork.Disconnect();
        SceneTransit.Instance.LoadScene(@"Title", 0.4f);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player) {
        Debug.Log($"A player(id: { player.ID }) has left room.");
    }

    private void updateOtherPlayerStatus() {
        if (PhotonNetwork.otherPlayers.Length > 0) {
            messageLabel.text = @"対戦相手が見つかりました！";
            statusLabel.text = @"あいて: " + PhotonNetwork.otherPlayers[0].NickName;
            cancelButton.interactable = false;
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player) {
        Debug.Log($"Joined player(id: { player.ID }) in this room.");
        updateOtherPlayerStatus();
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        updateOtherPlayerStatus();
    }

    public void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        statusLabel.text = @"あなた: " + PhotonNetwork.playerName;
        StartMatching();
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
