using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // Decrease room's left Player cnt
        PhotonNetwork.CurrentRoom.CustomProperties["leftCnt"]
            = (int)PhotonNetwork.CurrentRoom.CustomProperties["leftCnt"] - 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        
        // Set player's property to dead
        PhotonNetwork.LocalPlayer.CustomProperties["isAlive"] = false;
        PhotonNetwork.SetPlayerCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        
        player.GetComponent<PhotonView>().RPC("ActivateRagDoll", RpcTarget.All);
        
        InGameManager.gameResultDelegate?.Invoke();
    }
}


