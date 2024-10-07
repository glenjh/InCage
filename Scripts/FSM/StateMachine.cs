using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public PlayerState currState;

    public void Init(PlayerState initState)
    {
        currState = initState;
        currState.Enter();
    }

    public void ChangeState(PlayerState nextState)
    {
        currState?.Exit();
        currState = nextState;
        currState.Enter();
    }
}
