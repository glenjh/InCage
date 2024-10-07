using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Reference")]
    public Player player;
    public PhotonView PV;
    public GameObject camTarget;
    [Space] 
    
    [Header("Aim")] 
    public CinemachineVirtualCamera aimCam;
    public RectTransform crossHair;
    public float normalSize = 110f;
    public float maxSize = 250f;
    public float zoomSize = 70f;
    private float currSize;
    public float speed = 2f;
    [Space]
    
    [Header("Camera Settings")] 
    public float sensitivity = 1f;
    public float topClamp = 70f;
    public float bottomClamp = -30f;
    private float camTargetYaw;
    private float camTargetPitch;
        
    void Start()
    {
        if (PV.IsMine)
        {
            camTargetYaw = camTarget.transform.rotation.eulerAngles.y;
            player = GetComponent<Player>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void CamRotate()
    {
        DynamicCrossHair();
        
        camTargetYaw += player.look.x * sensitivity;
        camTargetPitch += player.look.y * sensitivity;

        camTargetYaw = ClampAngle(camTargetYaw, float .MinValue, float.MaxValue);
        camTargetPitch = ClampAngle(camTargetPitch, bottomClamp, topClamp);
        
        camTarget.transform.rotation = Quaternion.Euler(camTargetPitch, camTargetYaw, 0f);
        
        // Rotate Player
        float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, camTargetYaw, ref player.rotationVel,
            player.rotationSmoothTime);
        
        player.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    private void DynamicCrossHair()
    {
        if (!player.isAiming)
        {
            if (player.look != Vector2.zero)
            {
                currSize = Mathf.Lerp(currSize, maxSize, speed * Time.deltaTime);
            }
            else
            {
                currSize = Mathf.Lerp(currSize, normalSize, speed * Time.deltaTime);
            }
        }
        else
        {
            currSize = Mathf.Lerp(currSize, zoomSize, speed * Time.deltaTime);
        }

        crossHair.sizeDelta = new Vector2(currSize, currSize);
    }


    void ChangeCam()
    {
        if (aimCam.Priority > 30) return;
        if (player.isAiming)
        {
            aimCam.Priority = 20;
        }
        else
        {
            aimCam.Priority = 5;
        }
    }
    
    void LateUpdate()
    {
        if (PV.IsMine && player.currState != "DEAD")
        {
            CamRotate();
            ChangeCam();
        }
    }
    
    public void OnAiming(float camPos, float priority, float fov)
    {
        camTarget.transform.localPosition = new Vector3(0f, 0.8889999f, camPos);
        aimCam.Priority = (int) priority;
        aimCam.m_Lens.FieldOfView = fov;
    }
}
