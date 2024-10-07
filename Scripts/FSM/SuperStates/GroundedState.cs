using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class GroundedState : PlayerState
{
    protected bool isJumping;
    protected bool isCruching;
    
    public GroundedState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        if (isJumping && player.grounded && player.jumpTimeoutDelta <= 0.0f)
        {
            stateMachine.ChangeState(player.jumpState);
        }
        
        SetDir();
        ApplyGravity();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        moveVec = player.moveVec;
        isJumping = player.isJumping;
        isCruching = player.isCruching;
    }

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();

        Move();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
