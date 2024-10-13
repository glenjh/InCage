using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";

    [Header("UI Elements")] 
    [SerializeField] private GameObject buttons;
    [SerializeField] private TextMeshProUGUI connectingText;

    void Awake()
    {
        InitialConnect();
    }

    private void Start()
    {
        SoundManager.Instance.PlaySound2D("BGM",0,true,SoundType.BGM);
    }

    public void InitialConnect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            // Debug.Log("First Connect");
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "kr";
            
            PhotonNetwork.AutomaticallySyncScene = false;
            
            PhotonNetwork.GameVersion = gameVersion;
            
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            connectingText.enabled = false;
            buttons.SetActive(true);
            // Debug.Log("Already Connected");
        }
    }

    #region Connection
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        
        connectingText.enabled = false;
        buttons.SetActive(true);
        
        Debug.Log("Connected to Master Server");
    }
    
    public void ConnectToLobby()
    {
        LoadingSceneManager.instance.ChangeScene("Lobby");
    }
    #endregion
}
