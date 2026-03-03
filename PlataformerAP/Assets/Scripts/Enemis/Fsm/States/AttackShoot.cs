using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShoot<T> : State<T>
{
    private FSM<EnemyState> _fsm; 
    private Animator MyAnimator;
    private Transform Root;
    private Transform Player;

    public AttackShoot(Transform rt , Transform py, Animator anim)
    {
       Root = rt;
       Player = py;
       MyAnimator = anim;


    }

    public override void OnBegin()
    {
    }

    public override void OnEnd()
    {
        
    }

    public override void OnUpdate()
    {
        attack();
        if (Vector3.Distance(Root.position, Player.position) > 1.5f)
        {
            _fsm.SendInput(EnemyState.Follow);
        }
        else if (Vector3.Distance(Root.position, Player.position) < 0.5f)
        {
            _fsm.SendInput(EnemyState.Patrol);
        }
    }
    
    void attack()
    {
        MyAnimator.SetTrigger("Attack");
        MyAnimator.SetBool("walk", false);
        Root.LookAt(Player);
    }
    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

}
