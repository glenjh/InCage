using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI instance;
    
    [Header("Player Ref")]
    public Player player;
    
    [Header("UI Elements")]
    public GameObject bigMap;

    public TextMeshProUGUI playersCnt;

    #region Unity CallBacks
    void Awake()
    {
        instance = this;
    }
    
    void Update()
    {
        MapExpand();
    }

    void LateUpdate()
    {
        playersCnt.text = (int)PhotonNetwork.CurrentRoom.CustomProperties["leftCnt"]
                          + " / " + PhotonNetwork.CurrentRoom.PlayerCount;
    }
    #endregion

    void MapExpand()
    {
        if (player.isMapExpand)
        {
            bigMap.SetActive(true);
        }
        else
        {
            bigMap.SetActive(false);
        }
    }
}
