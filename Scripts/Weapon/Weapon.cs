using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Weapons;

namespace Weapons
{
    public enum Grade
    {
        Common,
        Rare,
        Epic,
        Legendary
    }
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public float damage;
    public float range;
    public float fireRate;
    public float fireTimer;
    public int weaponID;
    
    [Header("Weapon Effect")]
    public Transform muzzlePos;
    public GameObject muzzleEffect;
    
    Collider _collider;
    Rigidbody _rigidbody;
    
    [Header("Weapon Grade")]
    [SerializeField] private Weapons.Grade weaponGrade;
    [SerializeField] private List<GameObject> gradeEffect;
    
    [Header("Weapon Position")]
    public Transform leftHandPos;
    public Transform rightHandPos;
    public Transform defaultPos;
    
    [Header("Photon")]
    public PhotonView pv;
    
    public bool isBaseWeapon = false;
    
    public delegate void PickedDelegate(int weaponID);
    public event PickedDelegate PickedEvent;
    
    private Coroutine _rotateCoroutine;
    
    public int ammoMax;

    [SerializeField] private int _ammoCur;
    protected int ammoCur
    {
        get { return _ammoCur; }
        set
        {
            _ammoCur = value;
            AttackEvent?.Invoke(_ammoCur.ToString());
        }
    }

    public delegate void AttackDelegate(string curAmmo);
    public event AttackDelegate AttackEvent;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        //Init();
    }
    
    public virtual void Init(int id, Grade grade)
    {
        if(isBaseWeapon) return;
        weaponID = id;
        weaponGrade = grade;
        if (gradeEffect != null)
        {
            foreach (var effect in gradeEffect)
            {
                ParticleSystem.MainModule main = effect.GetComponent<ParticleSystem>().main;
                switch (weaponGrade)
                {
                    case Grade.Common:
                        main.startColor = Color.white;
                        break;
                    case Grade.Rare:
                        main.startColor = Color.blue;
                        break;
                    case Grade.Epic:
                        main.startColor = Color.magenta;
                        break;
                    case Grade.Legendary:
                        main.startColor = Color.yellow;
                        break;
                    default:
                        main.startColor = Color.white;
                        break;
                }
                
            }
            
        }
        _ammoCur = ammoMax;
        _rotateCoroutine = StartCoroutine(RotateCoroutine());
    }

    public void Update()
    {
        if(fireTimer < fireRate) {
            fireTimer += Time.deltaTime;
        }
        PickedEvent?.Invoke(this.weaponID);
    }

    public virtual void ReRoad()
    {
        //Debug.Log("ReRoad");
    }
    
    public virtual bool Attack()
    {
        if(fireTimer < fireRate) return false;
        //Debug.Log("Attack");
        fireTimer = 0;

        return true;
    }
    
    [PunRPC]
    public virtual void Picked()
    {
        _collider.enabled = false;
        foreach (var effect in gradeEffect)
        {
            effect.SetActive(false);
            
        }
        if(_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
        PickedEvent = null;
        //rigidbody.isKinematic = true;
    }

    [PunRPC]
    public virtual void Equip(ThirdPersonCam thirdPersonCam, WeaponControllerIK weaponControllerIK, ref TMPro.TextMeshProUGUI curAmmoText, ref TMPro.TextMeshProUGUI maxAmmoText, PhotonView photonView)
    {
        gameObject.transform.localPosition = defaultPos.localPosition;
        gameObject.transform.localRotation = defaultPos.localRotation;

        curAmmoText.text = ammoCur.ToString();
        maxAmmoText.text = ammoMax.ToString();
        
        weaponControllerIK.SetHandPos(leftHandPos, rightHandPos);
        
        pv = photonView;
    }
    
    [PunRPC]
    public void Drop()
    {
        gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0,0,60));
        _collider.enabled = true;
        foreach (var effect in gradeEffect)
        {
            effect.SetActive(true);
        }
        _rotateCoroutine = StartCoroutine(RotateCoroutine());
        //rigidbody.isKinematic = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
    
    //y축 회전 코루틴
    public IEnumerator RotateCoroutine()
    {
        while (true)
        {
            transform.Rotate(Vector3.up * (30 * Time.deltaTime),Space.World);
            yield return null;
        }
    }

    public virtual void Aiming()
    {
        
    }
}
