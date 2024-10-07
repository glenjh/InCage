using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon")]
public class WeaponSO : ScriptableObject
{
    public float damage;
    public float range;
    public float fireRate;
}
