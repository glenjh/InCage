using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Spawner : MonoBehaviour
{
    public GameObject[] characters;
    public List<Transform> spawnPoints;
    
    private PhotonView pv;
    private Vector3 spawnPoint; 

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && !IsSpawnDone())
        {
            SpawnPlayers();
            WeaponManager.instance.SpawnWeapons(PhotonNetwork.CurrentRoom.Players.Count);
        }
    }

    void SpawnPlayers()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (spawnPoints.Count == 0)
            {
                break;
            }
            
            int idx = Random.Range(0, spawnPoints.Count);
            spawnPoint = spawnPoints[idx].position;
            
            if (player.Value.IsLocal)
            {
                GameObject playerToSpawn = characters[(int)player.Value.CustomProperties["character"]];
                PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint, quaternion.identity);
            }
            else
            {
                pv.RPC("SetSpawnPoint", player.Value, spawnPoint, player.Value);
            }
            spawnPoints.RemoveAt(idx);
        }

        SetSpawnDone();
    }

    private bool IsSpawnDone()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("spawnDone"))
        {
            return (bool)PhotonNetwork.CurrentRoom.CustomProperties["spawnDone"];
        }
        return false;
    }

    private void SetSpawnDone()
    {
        // Hashtable roomCustomProperty = new Hashtable();
        var roomCustomProperty = PhotonNetwork.CurrentRoom.CustomProperties;
        roomCustomProperty["spawnDone"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomCustomProperty);
    }

    [PunRPC]
    void SetSpawnPoint(Vector3 pos, Photon.Realtime.Player player)
    {
        GameObject playerToSpawn = characters[(int)player.CustomProperties["character"]];
        PhotonNetwork.Instantiate(playerToSpawn.name, pos, quaternion.identity);
    }
}
