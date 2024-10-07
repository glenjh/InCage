using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : GroundedState
{
    public MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.isCruching = false;
        player.controller.center = new Vector3(0, player.normalCenter, 0);
        player.controller.height = player.normalHeight;
        targetSpeed = player.moveSpeed;
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        if (moveVec == Vector2.zero && !isCruching)
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (player.isSprinting)
        {
            stateMachine.ChangeState(player.sprintState);
        }
        
        if (isCruching)
        {
            stateMachine.ChangeState(player.CruchMoveState);
        }
    }
}
