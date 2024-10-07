using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance;

    [Header("Screens")] 
    public GameObject lobbyScreen;
    public GameObject roomScreen;
    public GameObject waitingPannel;

    [Header("Lobby UI")] 
    public Transform roomListParent;
    public RoomItemBtn roomItemPrefab;
    public TMP_InputField roomNameField;
    private List<RoomItemBtn> roomItemLists = new List<RoomItemBtn>();

    [Header("Room UI")] 
    public GameObject gameStartBtn;
    public TextMeshProUGUI roomNameText;
    public PlayerItem playerItemPrefab;
    public List<PlayerItem> playerItemLists = new List<PlayerItem>();
    public Transform playerItemParents;
    private Coroutine count;
    private bool isCounting = false;

    private float updateTime = 1f;  
    private float timer;

    #region Unity CallBacks
    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        PhotonNetwork.JoinLobby();
    }
    
    void LateUpdate()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("readyCnt"))
            {
                if (PhotonNetwork.IsMasterClient && 
                    ((int)PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] == PhotonNetwork.CurrentRoom.PlayerCount))
                {
                    gameStartBtn.SetActive(true);
                } 
                else
                {
                    gameStartBtn.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Lobby
    public override void OnJoinedLobby()
    {
        // Debug.Log("You are in a Lobby Now");
        base.OnJoinedLobby();
        
        waitingPannel.SetActive(false);
        
        Hashtable props = new Hashtable();
        props["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void BackToMain()
    {
        PhotonNetwork.LeaveLobby();
        LoadingSceneManager.instance.ChangeScene("Main");
    }
    #endregion
    
    #region About Room
    public void CreateRoom()
    {
        if (roomNameField.text.Length >= 1)
        {
            RoomOptions roomOption = new RoomOptions();
            roomOption.IsVisible = true;
            roomOption.IsOpen = true;
            roomOption.MaxPlayers = 8;
            roomOption.BroadcastPropsChangeToAll = true;
            
            PhotonNetwork.CreateRoom(roomNameField.text, roomOption, null);
        }
    }

    public override void OnCreatedRoom()
    {
        Hashtable customRoomProperties = new Hashtable();
        customRoomProperties["readyCnt"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        
        // Debug.Log(PhotonNetwork.CurrentRoom.Name + " Created");
    }
    
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        lobbyScreen.SetActive(false);
        roomScreen.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log(player.Value.NickName);
        }
        
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        
        Debug.Log("Failed to enter the Room. The room might be full or dose not exsist");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player enteredPlayer)
    {
        UpdatePlayerList();
    }
    
    public override void OnPlayerLeftRoom(Photon.Realtime.Player leftPlayer)
    {
        if ((bool)leftPlayer.CustomProperties["isReady"])
        {
            PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] =
                (int)PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] - 1;

            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
        
        UpdatePlayerList();
    }

    public void LeaveRoom()
    {
        gameStartBtn.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomScreen.SetActive(false);
        lobbyScreen.SetActive(true);
        
        Hashtable props = new Hashtable();
        props["isReady"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void OnStartBtnClicked()
    {
        if (!isCounting)
        {
            isCounting = true;
            
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            count = StartCoroutine(CountDown(3));
        }
        else
        {
            isCounting = false;

            if (count != null)
            {
                StopCoroutine(count);
                count = null;
            }
            gameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
            
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    IEnumerator CountDown(float f)
    {
        float timer = 3f;

        while (timer > 0)
        {
            gameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = ((int)timer + 1).ToString();
            timer -= Time.deltaTime;
            yield return null;
        }

        gameStartBtn.GetComponent<Button>().interactable = false;
        gameStartBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Entering";
        
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("spawnDone"))
        {
            PhotonNetwork.CurrentRoom.CustomProperties["spawnDone"] = null;
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }
            
        PhotonNetwork.LoadLevel("BattleZone");
    }
    
    public override void OnConnectedToMaster() // 방에서 떠나면 포톤 로비로 다시 접속되게 하기 위해서
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }
    #endregion

    #region Update Room Info
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= timer)
        {
            UpdateRoomListUI(roomList);
            timer = Time.time + updateTime;
        }
    }

    void UpdateRoomListUI(List<RoomInfo> list)
    {
        foreach (RoomItemBtn room in roomItemLists)
        {
            Destroy(room.gameObject);
        }
        roomItemLists.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItemBtn roomItem = Instantiate(roomItemPrefab, roomListParent);
            
            if (room.RemovedFromList || room.PlayerCount <= 0 || !room.IsOpen)
            {
                Destroy(roomItem.gameObject);
                roomItemLists.Remove(roomItem);
            }
            else
            {
                roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
                roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/8";
                roomItem.GetComponent<RoomItemBtn>().roomName = room.Name;
                
                roomItemLists.Add(roomItem);
            }
        }
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemLists)
        {
            Destroy(item.gameObject);
        }
        playerItemLists.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Photon.Realtime.Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParents);
            newPlayerItem.SetPlayerInfo(player.Value);

            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyChange();
            }
            
            playerItemLists.Add(newPlayerItem);
            
        }
    }
    #endregion
}
