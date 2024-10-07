using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class WeaponController : MonoBehaviour
{
    public PhotonView pv;
    public List<Weapon> weapons;
    public Weapon secWeapon;
    public Weapon fstWeapon;
    public Weapon baseWeapon;
    int curWeapon = 0;
    [SerializeField] private Player player;
    [SerializeField] private WeaponControllerIK weaponControllerIK;
    [SerializeField] private ThirdPersonCam thirdPersonCam;
    [SerializeField] private Transform weaponPos;
    public delegate void AttackDelegate();
    public event AttackDelegate AttackEvent;
    
    public delegate void ReloadDelegate();
    public event ReloadDelegate ReloadEvent;
    
    bool isReloading = false;
    
    [Header("UI")]
    public TextMeshProUGUI curAmmoText;
    public TextMeshProUGUI maxAmmoText;
    
    // Start is called before the first frame update
    void Start()
    {
        weapons[0].gameObject.SetActive(true);
        weapons[0].transform.parent = weaponPos;
        weapons[0].transform.localPosition = Vector3.zero;
        weapons[0].Picked();
        weapons[0].Equip(thirdPersonCam, weaponControllerIK, ref curAmmoText, ref maxAmmoText, pv);
        weapons[0].AttackEvent += ChangeCurAmmo;
        weapons[1] = null;
        weapons[2] = null;
        // baseWeapon.gameObject.SetActive(true);
        // baseWeapon.transform.parent = weaponPos;
        // baseWeapon.transform.localPosition = Vector3.zero;
        // //fstWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        // baseWeapon.Picked();
        // baseWeapon.Equip(thirdPersonCam, weaponControllerIK, ref curAmmoText, ref maxAmmoText, pv);
        // baseWeapon.AttackEvent += ChangeCurAmmo;
        
        AttackEvent += player.OnFire;
        ReloadEvent += player.OnReload;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && !isReloading)
            {
                pv.RPC("SwapWeaponRPC", RpcTarget.AllBuffered, 1);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2) && !isReloading)
            {
                pv.RPC("SwapWeaponRPC", RpcTarget.AllBuffered, 2);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3) && !isReloading)
            {
                pv.RPC("SwapWeaponRPC", RpcTarget.AllBuffered, 0);
            }

            if (Input.GetMouseButton(0) && !isReloading)
            {
                pv.RPC("OnFireRPC", RpcTarget.AllBuffered);
                AttackEvent?.Invoke();
            }

            if (Input.GetMouseButtonDown(1) && !isReloading)
            {
                weapons[curWeapon].Aiming();
            }

            if (Input.GetKeyDown(KeyCode.R) && !isReloading)
            {
                ReloadEvent?.Invoke();
            }
        }
    }
    
    [PunRPC]
    public void OnFireRPC()
    {
        weapons[curWeapon].Attack();
    }

    [PunRPC]
    private void SwapWeaponRPC(int type)
    {
        if(type == curWeapon || weapons[type] == null)
        {
            return;
        }
        weapons[curWeapon].gameObject.SetActive(false);
        curWeapon = type;
        weapons[curWeapon].gameObject.SetActive(true);
        ChangeWeapon(weapons[curWeapon], curWeapon);
        //무기 교체
    }
    
    [PunRPC]
    private void ChangeWeapon(Weapon weapon, int type)
    {
        weapons[type] = weapon;
        
        weapons[type].transform.parent = weaponPos;
        weapons[type].transform.localPosition = Vector3.zero;
        weapons[type].Picked();
        weapons[type].Equip(thirdPersonCam, weaponControllerIK, ref curAmmoText, ref maxAmmoText, pv);
        //방향을 초기화
        //fstWeapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [PunRPC]
    public void GetWeaponRPC(int weaponID)
    {
        if (weapons[1] != null)
        {
            if (weapons[2] == null)
            {
                weapons[2] = weapons[1];
                //weapons[2].gameObject.SetActive(false);
                weapons[curWeapon].gameObject.SetActive(false);
                curWeapon = 1;
            }
            else
            {
                if (curWeapon == 0) return;
                //바닥에 버림
                weapons[curWeapon].transform.parent = null;
                weapons[curWeapon].AttackEvent -= ChangeCurAmmo;
                weapons[curWeapon].Drop();
            }
        }
        else
        {
            weapons[curWeapon].gameObject.SetActive(false);
            curWeapon = 1;
        }
        
        ChangeWeapon(WeaponManager.instance.weaponDict[weaponID], curWeapon);
        //구독 해제
        weapons[curWeapon].PickedEvent -= GetWeapon;
        weapons[curWeapon].AttackEvent += ChangeCurAmmo;
    }

    private void GetWeapon(int weaponID)
    {
        if (Input.GetKeyDown(KeyCode.E) && pv.IsMine)
        {
            pv.RPC("GetWeaponRPC", RpcTarget.AllBuffered, weaponID);
            Debug.Log(WeaponManager.instance.weaponDict[weaponID].name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            other.GetComponent<Weapon>().PickedEvent += GetWeapon;
            Debug.Log(other.name + " Enter");
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            other.GetComponent<Weapon>().PickedEvent -= GetWeapon;
            Debug.Log(other.name + " Exit");
        }
    }
    
    public void OnReload()
    {
        isReloading = true;
    }
    
    public void OnReloadEnd()
    {
        isReloading = false;
        weapons[curWeapon].ReRoad();
    }

    public void ChangeCurAmmo(string ammo)
    {
        curAmmoText.text = ammo;
    }
}
