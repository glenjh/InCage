using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprintState : GroundedState
{
    public SprintState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.isCruching = false;
        player.controller.center = new Vector3(0, player.normalCenter, 0);
        player.controller.height = player.normalHeight;
        targetSpeed = player.sprintSpeed;
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        if (isCruching)
        {
            stateMachine.ChangeState(player.CruchMoveState);
        }

        if (!player.isSprinting && !isCruching)
        {
            stateMachine.ChangeState(player.moveState);
        }

        if (moveVec == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
