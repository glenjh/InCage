using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class BlueZoneShrinkAndDamage : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    
    [Header("Renderders")]
    public LineRenderer currZoneRenderer;
    public LineRenderer nextZoneRenderer;
    [Space] 
    
    [Header("BlueZone Settings")] 
    public GameObject blueZone;
    [Space]
    
    public Material blueZoneMat;
    public Vector2 tilingVec;
    private int initTilingTarget = 30;
    private int tilingIdx;
    public List<int> tilingVals;
    [Space]
    
    public Vector3 currCircleCenter;
    public Vector3 nextCircleCenter;
    [Space]
    public float currRadius;
    public float nextRadius;

    public List<int> zoneTims;
    [Space]
    
    [Header("Damage Settings")]
    public bool inZone = true;

    public List<int> tickDamages;
    private float tickRate = 1f;
    private int damageIdx;
    
    [Header("Full Screen Effects")]
    public Material fullScreenEffectMat;
    
    private float vignetteSetting = 1.5f;
    
    #region Private Vals
    
    private bool isShrinking;
    private bool nextCenterFound = false;
    
    private float distanceToMove;
    private float timePassed;

    private int zoneTimeIdx;
    private int segments = 64;
    private int shrinkTime = 30; //30
    private int count = 0;
    private int countDownPrecall = 10;
    private CapsuleCollider col;
    
    #endregion
    
    void Start()
    {
        tilingVec = blueZoneMat.GetVector("_HexsTiling");

        currRadius = 450f;
        nextRadius = currRadius / 2;
        blueZone.transform.localScale = new Vector3((currRadius * 0.01f), blueZone.transform.localScale.y,
            (currRadius * 0.01f));
        
        currCircleCenter = gameObject.transform.position;
        nextCircleCenter = currCircleCenter;
        timePassed = Time.deltaTime;
        
        col = GetComponent<CapsuleCollider>();
        
        // activate "ZoneDamage" in every tickRate seconds
        if (PV.IsMine)
        {
            InvokeRepeating("ZoneDamage", 1f, tickRate);
        }
    }

    public override void OnDisable()
    {
        blueZoneMat.SetVector("_HexsTiling", new Vector2(170, initTilingTarget));
        fullScreenEffectMat.SetFloat("_VignetteIntensity", 0f);
    }

    void Update()
    {
        Shrink();
    }

    #region Blue Zone Shrink
    void Shrink()
    {
        Draw(currZoneRenderer, segments, currRadius, transform.position);
        
        blueZone.transform.localScale = new Vector3(currRadius * 0.01f, blueZone.transform.localScale.y, currRadius * 0.01f);

        if (isShrinking)
        {
            if (!nextCenterFound)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Vector3 newPoint = FindNewCenter(currCircleCenter, currRadius, nextRadius);
                    PV.RPC("SetCenterPoint", RpcTarget.AllViaServer, newPoint);
                }
                
                distanceToMove = Vector3.Distance(currCircleCenter, nextCircleCenter);
                nextCenterFound = (nextCircleCenter != currCircleCenter);
            }

            if (nextCenterFound)
            {
                Draw(nextZoneRenderer, segments, nextRadius, nextCircleCenter);
            }
            
            float step = distanceToMove / shrinkTime * Time.deltaTime; // 프레임마다 이동할 양을 계산
            
            transform.position = Vector3.MoveTowards(transform.position, nextCircleCenter, step);
            currRadius = Mathf.MoveTowards(currRadius, nextRadius, (nextRadius / shrinkTime) * Time.deltaTime);
            
            // Change the tiling of the Bluezone Mat
            tilingVec.y = Mathf.MoveTowards(tilingVec.y, tilingVals[tilingIdx], (nextRadius * 0.01f / shrinkTime) * 2.5f * Time.deltaTime);
            blueZoneMat.SetVector("_HexsTiling", tilingVec);
            
            if (currRadius == nextRadius)
            {
                nextZoneRenderer.enabled = false;
                timePassed = Time.deltaTime;
                isShrinking = false;
                nextCenterFound = false;
                currCircleCenter = nextCircleCenter;
            }
        }
        else
        {
            timePassed += Time.deltaTime;
        }

        if ((int)timePassed > zoneTims[zoneTimeIdx])
        {
            nextRadius = currRadius * 0.5f;
            isShrinking = true;
            timePassed = Time.deltaTime;

            if (NextZoneTime() < 0)
            {
                isShrinking = false;
            }
        }

        if (timePassed > zoneTims[zoneTimeIdx] - countDownPrecall)
        {
            if (zoneTims[zoneTimeIdx] - (int)timePassed != count)
            {
                count = Mathf.Clamp(zoneTims[zoneTimeIdx] - (int)timePassed, 1, 1000);
                // Debug.Log("Shrinking in "+ count + " seconds");
            }
        }
    }
    
    Vector3 FindNewCenter(Vector3 currCenter, float currRadius, float nextRadius)
    {
        Vector3 newPoint;

        Vector2 randomPoint = new Vector2(Random.Range(-nextRadius, nextRadius),
            Random.Range(-nextRadius, nextRadius));

        newPoint = currCenter + new Vector3(randomPoint.x, 0, randomPoint.y);
        return newPoint;
    }

    [PunRPC]
    void SetCenterPoint(Vector3 newCenter)
    {
        nextCircleCenter = newCenter;
    }

    int NextZoneTime()
    {
        if (zoneTimeIdx < zoneTims.Count - 1)
        {
            tilingIdx += 1;
            zoneTimeIdx += 1;
            damageIdx += 1;
            return zoneTimeIdx;
        }
        else
        {
            return -1;
        }
    }
    
    void Draw(LineRenderer renderer, int segments, float radius, Vector3 centerPoint)
    {
        renderer.enabled = true;
        renderer.positionCount = segments + 1;
        renderer.useWorldSpace = true;

        float angle = 20f;
        
        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            
            renderer.SetPosition(i, new Vector3(centerPoint.x + x, 500, centerPoint.z + z));

            angle += (360f / segments);
        }
    }
    #endregion

    #region Blue Zone Damage

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (!inZone)
            {
                fullScreenEffectMat.SetFloat("_VignetteIntensity", 0f);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            inZone = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            inZone = false;
            
            fullScreenEffectMat.SetFloat("_VignetteIntensity", vignetteSetting);
        }
    }

    void ZoneDamage()
    {
        if (!inZone)
        {
            Debug.Log("You got hot by : " + tickDamages[damageIdx]);
        }
    }
    #endregion
}