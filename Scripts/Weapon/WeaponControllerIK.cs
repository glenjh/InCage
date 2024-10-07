using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllerIK : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform leftHandPos;
    [SerializeField] private Transform rightHandPos;
    
    [SerializeField] private Transform gunPivot;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void SetHandPos(Transform leftHandPos, Transform rightHandPos)
    {
        this.leftHandPos = leftHandPos;
        this.rightHandPos = rightHandPos;
    }

    private void LateUpdate()
    {
    }

    public void OnAnimatorIK(int layerIndex)
    {
        if(leftHandPos == null || rightHandPos == null) return;
         gunPivot.position = anim.GetIKHintPosition(AvatarIKHint.RightElbow);
        
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPos.rotation);
        
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        
        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandPos.rotation);   
    }
}
