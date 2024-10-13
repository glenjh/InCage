using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;
    public Dictionary<int, Item> itemDict = new Dictionary<int, Item>();
    public Item[] items;
    public PhotonView PV;
    //플레이어당 아이템 생성 가중치
    [SerializeField] private int[] itemWeight;
    public Transform itemParent;
    
    public Vector3 center;
    public float range;

    private void Awake()
    {
        Init();
        //PV = GetComponent<PhotonView>();
    }
    
    public void Init()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SpawnItems(int playerCnt)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            for (var i = 0; i < itemWeight[playerCnt]; i++)
            {
                while (true)
                {
                    Vector3 randomPoint = center + Random.insideUnitSphere * range;
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                    {
                        var pos = hit.position + Vector3.up * 0.5f;
                        PV.RPC("SpawnItemRPC", RpcTarget.All, pos, i, Random.Range(0, items.Length));
                        break;
                    }
                }
            }
        }
    }

    [PunRPC]
    private void SpawnItemRPC(Vector3 pos, int key, int itemID)
    {
        var rot = Quaternion.Euler(0, 0, 60);
        Item item = Instantiate(items[itemID], pos, rot);
        item.Init(key);
        item.transform.parent = itemParent;
        itemDict.Add(key, item);
    }
}
