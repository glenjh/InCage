using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerState
{
    private bool isCruching;
    
    public JumpState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.isCruching = false;
        player.controller.center = new Vector3(0, player.normalCenter, 0);
        player.controller.height = player.normalHeight;
        targetSpeed = player._speed * 0.95f;
        player.verticalVel = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        player.anim.SetBool(player.animIDJump, true);
        player.StartCoroutine(Check());
    }

    IEnumerator Check()
    {
        player.groundCheckAble = false;

        yield return new WaitForSecondsRealtime(0.1f);

        player.groundCheckAble = true;
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        ApplyGravity();
        SetDir();
        
        if (player.grounded && player.groundCheckAble)
        {
            if (player.moveVec == Vector2.zero)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.moveState);
            }
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveVec = player.moveVec;
        isCruching = player.isCruching;
    }

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();

        if (player._speed == 0 && moveVec != Vector2.zero)
        {
            targetSpeed = player.moveSpeed * 0.9f;
        }
        
        Move();
    }
}
