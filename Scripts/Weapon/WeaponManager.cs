using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance = null;
    public Dictionary<int, Weapon> weaponDict = new Dictionary<int, Weapon>();
    public Weapon[] weapons;
    public PhotonView PV;
    //플레이어당 무기 생성 가중치
    [SerializeField] private int[] weaponWeight;
    //무기 등급 가중치
    [SerializeField] private List<int> gradeWeight;
    public Vector3 center;
    public float range;

    public Transform weaponParent;

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

    public void SpawnWeapons(int playerCnt)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            float totalWeight = 0;
            foreach (var variable in gradeWeight)
            {
                totalWeight += variable;
            }
            
            for (var i = 0; i < weaponWeight[playerCnt]; i++)
            {
                var temp = Random.Range(0, totalWeight);
                for (var j = 0; j < gradeWeight.Count; j++)
                {
                    temp -= gradeWeight[j];
                    if (temp <= 0)
                    {
                        while (true)
                        {
                            Vector3 randomPoint = center + Random.insideUnitSphere * range;
                            NavMeshHit hit;
                            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                            {
                                var pos = hit.position + Vector3.up * 0.5f;
                                PV.RPC("SpawnWeaponRPC", RpcTarget.All, pos, i, j, Random.Range(0, weapons.Length));
                                break;
                            }
                        }
                        break;
                    }
                }
                
            }
        }
    }

    [PunRPC]
    private void SpawnWeaponRPC(Vector3 pos, int key, int grade, int weaponID)
    {
        var rot = Quaternion.Euler(0, 0, 60);
        Weapon weapon = Instantiate(weapons[weaponID], pos, rot);
        weapon.Init(key, (Weapons.Grade)grade);
        weapon.transform.parent = weaponParent;
        weaponDict.Add(key, weapon);
    }
}
