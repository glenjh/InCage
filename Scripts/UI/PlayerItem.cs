using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    [Header("Info")]
    public TextMeshProUGUI playerName;

    [Header("Private Settings")] 
    public GameObject masterIcon;
    public Image backgroundImage;
    public Color myColor;
    public GameObject leftBtn;
    public GameObject rightBtn;
    public Image characterImage;
    public Sprite[] characters;
    [Space]
    
    public Button readyBtn;
    public Color readyColor;
    public Color notReadyColor;
    
    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    Photon.Realtime.Player player;

    void Start()
    {
        playerProperties["isAlive"] = true;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void SetPlayerInfo(Photon.Realtime.Player _player)
    {
        readyBtn.GetComponent<Image>().color = (bool)_player.CustomProperties["isReady"] ? readyColor : notReadyColor;
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(_player);
    }

    public void ApplyChange()
    {
        backgroundImage.color = myColor;
        leftBtn.SetActive(true);
        rightBtn.SetActive(true);
        readyBtn.interactable = true;
    }

    public void OnLeftBtnClicked()
    {
        if ((int)playerProperties["character"] == 0)
        {
            playerProperties["character"] = characters.Length - 1;
        }
        else
        {
            playerProperties["character"] = (int)playerProperties["character"] - 1;
        }
         
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnRightBtnClicked()
    {
        if ((int)playerProperties["character"] == characters.Length - 1)
        {
            playerProperties["character"] = 0;
        }
        else
        {
            playerProperties["character"] = (int)playerProperties["character"] + 1;
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void OnReadyBtnClicked()
    {
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"])
        {
            playerProperties["isReady"] = false;
            PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] = (int)PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] - 1;
        }
        else
        {
            playerProperties["isReady"] = true;
            PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] = (int)PhotonNetwork.CurrentRoom.CustomProperties["readyCnt"] + 1;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void UpdatePlayerItem(Photon.Realtime.Player _player)
    {
        // Master CLinet Distinguish
        if (_player.IsMasterClient)
        {
            masterIcon.SetActive(true);
        }
        else
        {
            masterIcon.SetActive(false);
        }
        
        // Set Character of Player
        if (_player.CustomProperties.ContainsKey("character"))
        {
            characterImage.sprite = characters[(int)_player.CustomProperties["character"]];
            playerProperties["character"] = (int)_player.CustomProperties["character"];
        }
        else
        {
            playerProperties["character"] = 0;
        }
        
        // Is player ready or not
        if (_player.CustomProperties.ContainsKey("isReady"))
        {
            readyBtn.GetComponent<Image>().color = (bool)_player.CustomProperties["isReady"] ? readyColor : notReadyColor;
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable hashtable)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }
}
