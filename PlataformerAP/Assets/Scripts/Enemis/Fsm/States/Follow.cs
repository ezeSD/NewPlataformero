
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follow<T> : State<T>
{
    private FSM<EnemyState> _fsm;
    
    private float steeringForce = 0.1f;
    private float speed = 3f;
    private float stoppingDistance = 10f;
    private float distanceToPlayer = 10f;
    private Transform Root;
    private Transform Player;
    private Animator MyAnimator;
    private TypeEnemy typeEnemy;

    #region Vectores 3
    private Vector3 desired = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 steering = Vector3.zero;
    private Vector3 dir = Vector3.zero;
    #endregion
    public Follow(Transform Rt, Transform Py, Animator Anim,float sp, float st, TypeEnemy enm , float stD)
    {
       Root = Rt;
       Player = Py;
       MyAnimator = Anim;
       speed = sp;
       steeringForce = st;
         typeEnemy = enm;
         stoppingDistance = stD;

    }

    public override void OnBegin()
    {
        if (typeEnemy == TypeEnemy.Range || typeEnemy == TypeEnemy.Fly)
        {
            _fsm.SendInput(EnemyState.Attack);
        }

    }

    public override void OnEnd()
    {


    }
  
    public override void OnUpdate()
    {
        if(typeEnemy == TypeEnemy.Melee)
        {
            FollowBehavior();
        }
            changeState();


    }


    void changeState()
    {
        if (Vector3.Distance(Root.position, Player.position) <= distanceToPlayer)
        {
            _fsm.SendInput(EnemyState.Attack);
        }
        else if (Vector3.Distance(Root.position, Player.position) > distanceToPlayer * 2)
        {
            _fsm.SendInput(EnemyState.Patrol);
        }
    }


    void FollowBehavior()
    {
        float distanceToPlayer = Vector3.Distance(Root.position, Player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            dir = Player.position - Root.position;
            desired = dir.normalized * speed;
            steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, steeringForce);
            velocity = Vector3.ClampMagnitude(velocity + steering, speed);
            Root.position += velocity * Time.deltaTime;
            MyAnimator.SetBool("walk", true);
            Root.LookAt(Player.position);
        }
        else
        {
            _fsm.SendInput(EnemyState.Attack);
        }
    }

    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    
}



