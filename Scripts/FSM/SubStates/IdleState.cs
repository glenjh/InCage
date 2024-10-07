using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : GroundedState
{
    public IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.isCruching = false;
        player.isSprinting = false;
        player.controller.center = new Vector3(0, player.normalCenter, 0);
        player.controller.height = player.normalHeight;
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();

        if (moveVec != Vector2.zero)
        {
            stateMachine.ChangeState(player.moveState);
        }
        else
        {
            if (isCruching)
            {
                stateMachine.ChangeState(player.CruchIdleState);
            }
        }
    }
}
