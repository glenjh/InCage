using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGameManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
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
}
