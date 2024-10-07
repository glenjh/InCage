using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Pistol : Weapon
{
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
            
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, range, layerMask))
            {
                // && !hitInfo.collider.gameObject.GetComponent<PhotonView>().IsMine
                if(hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    hitInfo.collider.gameObject.GetComponent<PhotonView>()
                        .RPC("TakeDamageRPC", RpcTarget.AllBuffered, damage);
                }
                Instantiate(hitEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                // var damageable = hitInfo.transform.GetComponent<IDamageable>();
                // if (damageable != null)
                // {
                //     damageable.TakeDamage(damage, pv);
                // }
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
