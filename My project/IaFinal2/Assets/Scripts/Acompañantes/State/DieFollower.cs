using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFollower<T> : State<T>
{
    FSM<EnemyState> _fsm;
    public override void OnBegin()
    {
        throw new System.NotImplementedException();
    }

    public override void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }
}
