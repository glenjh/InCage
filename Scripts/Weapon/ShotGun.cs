using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ShotGun : Weapon
{
    public int pelletCount;
    public float spread;
    public GameObject hitEffect;
    
    [PunRPC]
    public override bool Attack()
    {
        if(!base.Attack())
        {
            return false;
        }
        if (ammoCur > 0)
        {
            Instantiate(muzzleEffect, muzzlePos.position, muzzlePos.rotation);
            //player를 제외
            int layerMask = (-1) - (1 << LayerMask.NameToLayer("NonTarget"));
            
            for (int i = 0; i < pelletCount; i++)
            {
                //스크린을 기준으로 발사
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
                ray.direction = Quaternion.Euler(Random.Range(-spread, spread), Random.Range(-spread, spread), Random.Range(-spread, spread)) * ray.direction;
                
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, range, layerMask))
                {
                    if(hitInfo.collider.gameObject.CompareTag("Player"))
                    {
                        hitInfo.collider.gameObject.GetComponent<PhotonView>()
                            .RPC("TakeDamageRPC", RpcTarget.AllBuffered, damage);
                    }
                    Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                }
            }
            
            ammoCur--;
            return true;
        }
        else
        {
            ReRoad();
            return false;
        }
    }

    public override void ReRoad()
    {
        base.ReRoad();
        ammoCur = ammoMax;
    }
}