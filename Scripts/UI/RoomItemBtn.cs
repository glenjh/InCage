using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class RoomItemBtn : MonoBehaviour
{
    public string roomName;

    public void OnButtonPressed()
    {
        LobbyManager.instance.JoinRoom(roomName);
    }
}
