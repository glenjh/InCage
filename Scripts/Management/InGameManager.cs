using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public delegate void GameResultDelegate();
    public static GameResultDelegate gameResultDelegate;

    [SerializeField] private PhotonView pv;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        gameResultDelegate += EndGame;
        
        var roomCustomProperty = PhotonNetwork.CurrentRoom.CustomProperties;
        roomCustomProperty["leftCnt"] = PhotonNetwork.CurrentRoom.PlayerCount;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperty);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player leftPlayer)
    {
        if (IsAlive(leftPlayer))
        {
            var roomCustomProperty = PhotonNetwork.CurrentRoom.CustomProperties;
            roomCustomProperty["leftCnt"] = (int)roomCustomProperty["leftCnt"] - 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperty);
        }
    }

    bool IsAlive(Photon.Realtime.Player leftPlayer)
    {
        return (bool)leftPlayer.CustomProperties["isAlive"];
    }

    public void EndGame()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            pv.RPC("EndCheckRPC", player.Value);
        }
    }

    [PunRPC]
    void EndCheckRPC()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if(!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isAlive"])
        {
            loseCanvas.SetActive(true);
        }
        
        if ((int)PhotonNetwork.CurrentRoom.CustomProperties["leftCnt"] == 1)
        {
            if((bool)PhotonNetwork.LocalPlayer.CustomProperties["isAlive"])
            {
                winCanvas.SetActive(true);
            }
        }
    }
}
