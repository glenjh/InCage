using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Player player;
    protected StateMachine stateMachine;
    
    protected Vector2 moveVec;
    protected float targetSpeed;
    
    public string stateName;

    public PlayerState(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        //Debug.Log(stateName);
    }
    
    public virtual void HandleInput() {}

    public virtual void LogicalUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            stateMachine.ChangeState(player.deadState);
        }
    }
    
    protected void SetDir()
    {
        float currentHorizontalSpeed =
            new Vector3(player.controller.velocity.x, 
                0.0f, 
                player.controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            player._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * player.speedChangeRate);
            
            player._speed = Mathf.Round(player._speed * 1000f) / 1000f;
        }
        else
        {
            player._speed = targetSpeed;
        }

        player.animBlendX = Mathf.Lerp(player.animBlendX, targetSpeed * moveVec.x, Time.deltaTime * player.speedChangeRate);
        if (Mathf.Abs(player.animBlendX) < 0.01f) player.animBlendX = 0f;
        
        player.animBlendY = Mathf.Lerp(player.animBlendY, targetSpeed * moveVec.y, Time.deltaTime * player.speedChangeRate);
        if (Mathf.Abs(player.animBlendY) < 0.01f) player.animBlendY = 0f;
        
        player.anim.SetFloat(player.animIDX, player.animBlendX);
        player.anim.SetFloat(player.animIDY, player.animBlendY);
    }

    public virtual void PhysicalUpdate() {}
    
    protected void Move()
    {
        Vector3 inputDir = new Vector3(moveVec.x, 0f, moveVec.y).normalized;

        if (moveVec != Vector2.zero)
        {
            player.targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg +
                                    player.mainCam.transform.eulerAngles.y;
        }
        
        Vector3 targetDirection = Quaternion.Euler(0.0f, player.targetRotation, 0.0f) * Vector3.forward;

        // move the player
        player.controller.Move(targetDirection.normalized * (player._speed * Time.deltaTime) +
                               new Vector3(0.0f, player.verticalVel, 0.0f) * Time.deltaTime);
    }
    
    protected void ApplyGravity()
    {
        if (player.grounded && player.groundCheckAble)
        {
            player.fallTimeoutDelta = player.fallTimeOut;
            
            player.anim.SetBool(player.animIDCruched, player.isCruching);
            player.anim.SetBool(player.animIDJump, false);
            player.anim.SetBool(player.animIDFreeFall, false);
            
            if (player.verticalVel < 0.0f)
            {
                player.verticalVel = -2f;
            }

            if (player.jumpTimeoutDelta >= 0.0f)
            {
                player.jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            player.jumpTimeoutDelta = player.jumpTimeOut;

            if (player.fallTimeoutDelta >= 0.0f)
            {
                player.fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                player.anim.SetBool(player.animIDFreeFall, true);
            }
        }

        player.isJumping = false;

        if (player.verticalVel < player.terminalVel)
        {
            player.verticalVel += player.gravity * Time.deltaTime;
        }
    }
    
    public virtual void Exit() {}
}
