using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using Random = UnityEngine.Random;


public class CageDetruction : MonoBehaviour
{
    public PhotonView pv;
    public GameObject normalCage;
    public GameObject brokenCage;
    private GameObject fragObj;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Destruct()
    {
        pv.RPC("CageOff", RpcTarget.AllBufferedViaServer);

        fragObj = PhotonNetwork.Instantiate(brokenCage.name, new Vector3(transform.position.x, 
                                                                                transform.position.y - 1.6f, 
                                                                                transform.position.z), quaternion.identity);

        foreach (Transform t in fragObj.transform)
        {
            var rb = t.GetComponent<Rigidbody>();
            
            rb.AddExplosionForce(Random.Range(0.05f, 0.1f), transform.position, 0.5f);
        }

        StartCoroutine("Delete");
    }

    public IEnumerator Delete()
    {
        yield return new WaitForSecondsRealtime(2f);
        
        PhotonNetwork.Destroy(fragObj);
    }

    [PunRPC]
    void CageOff()
    {
        normalCage.SetActive(false);
    }
}
