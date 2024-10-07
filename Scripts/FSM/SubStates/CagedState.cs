using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CagedState : PlayerState
{
    private bool grounded;
    
    public CagedState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // Ragdoll OFF
        player.controller.enabled = true;
        player.anim.enabled = true;
        foreach (Collider col in player.ragdollCols)
        {
            col.enabled = false;
        }
        foreach(Rigidbody rb in player.ragdollRigs)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        grounded = player.grounded;
    }

    public override void LogicalUpdate()
    {
        base.LogicalUpdate();
        
        ApplyGravity();

        if (grounded)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void PhysicalUpdate()
    {
        base.PhysicalUpdate();
        
        player.controller.Move(new Vector3(0.0f, player.verticalVel, 0.0f) * Time.deltaTime);
    }

    public override void Exit()
    {
        base.Exit();
        
        player.cage.GetComponent<CageDetruction>().Destruct();
    }
}
