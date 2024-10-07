using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float damage;
    public float lifeTime;
    public float distance;
    public LayerMask whatIsSolid;
    public GameObject destroyEffect;
    public GameObject hitEffect;
    public GameObject player;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.right * speed);
        Destroy(gameObject, lifeTime);
    }
}