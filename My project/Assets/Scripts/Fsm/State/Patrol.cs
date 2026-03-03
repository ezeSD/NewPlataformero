using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol<T> : State<T>
{
    private FSM<EnemyState> _fsm; // Referencia a la FSM
    Vector3 desired = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Vector3 steering = Vector3.zero;
    Vector3 dir = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    Transform[] points;
    Transform Propio;
    Transform target;
    int destPoint = 0;
    float rangeFollow = 2f;
    float range =  5f;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float steeringForce = 0.1f;
    [SerializeField] float wanderDistance = 1f;
    public Patrol(Transform transform, Transform[] Weapons, int Range,int RangeFollow, Transform Target)
    {
        points = Weapons;
        Propio = transform;
        range = Range;
        rangeFollow = RangeFollow;
        target = Target;
    }

    public override void OnBegin()
    {
        targetPos = Propio.transform.position;

    }

    public override void OnEnd()
    {
       
    }

    public override void OnUpdate()
    {
       PatrolBehavior();
    }

    void PatrolBehavior()
    {
        if (points.Length > 0 )
        {
            dir = targetPos - Propio.transform.position;
            if (dir.magnitude < wanderDistance)
            {
                destPoint = (destPoint + 1) % points.Length;
                targetPos = points[destPoint].position;
                Propio.transform.LookAt(targetPos);
            }
            desired = dir.normalized * moveSpeed;
            steering = desired - velocity;
            steering = Vector3.ClampMagnitude(steering, steeringForce);
            velocity = Vector3.ClampMagnitude(velocity + steering, moveSpeed);
            Propio.transform.position += velocity * Time.deltaTime;
            Vector3 moveDir = velocity.normalized;

            float tr = Vector3.Distance(Propio.transform.position, target.position);
            if (tr <= range)
            {      
                StopPatrol(EnemyState.Desperate);
            }
            else if (tr <= rangeFollow)
            {
                StopPatrol(EnemyState.InRange);
            }

        }
    }

    void StopPatrol(EnemyState enemy)
    {
       _fsm.SendInput(enemy);
    }
    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }



}


