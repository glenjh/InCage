using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruchMoveState : GroundedState
{
    public CruchMoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.isSprinting = false;
        player.controller.center = new Vector3(0, player.cruchedCenter, 0);
        player.controller.height = player.cruchedHieght;
        targetSpeed = player.cruchSpeed;
    }
    
    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        if (!isCruching)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else
        {
            if (moveVec == Vector2.zero)
            {
                stateMachine.ChangeState(player.CruchIdleState);
            }
            
            if (player.isSprinting)
            {
                stateMachine.ChangeState(player.sprintState);
            }
        }
    }
}
