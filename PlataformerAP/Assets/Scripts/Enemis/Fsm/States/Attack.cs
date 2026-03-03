using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeEnemy
{
    Range,
    Fly,
    Melee
}


public class Attack<T> : State<T>
{
    private FSM<EnemyState> _fsm; 
    private Animator MyAnimator;
    private Transform Root;
    private Transform Player;

    private float distanceToPlayer = 1.5f;
    EnemyArmor RangeEnemy;
    FlyEnemyArmor FlyEnemy;
    TypeEnemy typeEnemy;
    public Attack(Transform rt , Transform py, Animator anim, TypeEnemy te, EnemyArmor rangeEnemy, FlyEnemyArmor flyEnemy)
    {
        Root = rt;
        Player = py;
        MyAnimator = anim;
        typeEnemy = te;
        RangeEnemy = rangeEnemy;
        FlyEnemy = flyEnemy;
    }

    public override void OnBegin()
    {
        if (typeEnemy == TypeEnemy.Fly)
        {
            attackRangeFly();
        }
    }

    public override void OnEnd()
    {
        
    }

    public override void OnUpdate()
    {
        attack();
        changeState();
    }
    



    void attack()
    {
        if (typeEnemy == TypeEnemy.Fly)
        {
            attackRangeFly();
        }
        else if (typeEnemy == TypeEnemy.Range)
        {
            attackRangeFloor();
        }
        else if (typeEnemy == TypeEnemy.Melee)
        {
            attackMele();
        }
    }


    void attackRangeFly()
    {
        FlyEnemy.ColdownShoot();
        _fsm.SendInput(EnemyState.Patrol);
        Debug.Log("mande el input de ataque");
    }

    void attackRangeFloor()
    {
        MyAnimator.SetTrigger("Attack");
        MyAnimator.SetBool("walk", false);
        Root.LookAt(Player);
    }
    void attackMele()
    {
        if(MyAnimator != null)
        {
            MyAnimator.SetTrigger("Attack");
            MyAnimator.SetBool("walk", false);

        }

        Root.LookAt(Player);

    }


    void changeState()
    {
        if (Vector3.Distance(Root.position, Player.position) > 1.5f)
        {
            _fsm.SendInput(EnemyState.Follow);
        }
        else if (Vector3.Distance(Root.position, Player.position) < 0.5f)
        {
            _fsm.SendInput(EnemyState.Patrol);
        }
    }

    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

}
