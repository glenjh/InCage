using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Rifle : Weapon
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
                // 공격자만 임팩트 이펙트를 생성하게 수정
                if (PhotonView.Get(this).IsMine) // IsMine을 사용하여 로컬 플레이어만 처리
                {
                    if (hitInfo.collider.gameObject.CompareTag("Player") &&
                        !hitInfo.collider.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        if (hitInfo.collider.gameObject.GetComponent<Player>().curShield > 0)
                        {
                            PhotonNetwork.Instantiate("ShieldImpact", hitInfo.point,
                                Quaternion.LookRotation(hitInfo.normal));
                        }
                        else
                        {
                            PhotonNetwork.Instantiate("HealthImpact", hitInfo.point,
                                Quaternion.LookRotation(hitInfo.normal));
                        }
                    }
                    else
                    {
                        PhotonNetwork.Instantiate("hitImpact", hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    }
                }

                // 데미지는 모든 클라이언트에 동기화
                if (hitInfo.collider.gameObject.CompareTag("Player") &&
                    !hitInfo.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hitInfo.collider.gameObject.GetComponent<PhotonView>()
                        .RPC("TakeDamageRPC", RpcTarget.AllBuffered, damage);
                }
                SoundManager.Instance.PlaySound3D("Bullet Impact",hitInfo.transform,0,false,SoundType.EFFECT,false,0,10f);
            }

            ammoCur--;
            SoundManager.Instance.PlaySound3D("Shot",muzzlePos);
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