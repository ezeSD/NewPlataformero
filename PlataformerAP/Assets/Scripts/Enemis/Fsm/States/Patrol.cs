
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Patrol<T> : State<T>
{
    private FSM<EnemyState> _fsm;


    private Transform Root;
    private Animator MyAnimator;
    private Transform[] Waypoints;


    private float speed = 2f;
    private float steeringForce = 0.1f;
    private float wanderDistance = 1f;
    private int direction = 1;
    private int destPoint = 0;

  private TypeEnemy typeEnemy;

    #region Vectores 3
    private Vector3 desired = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 steering = Vector3.zero;
    private Vector3 dir = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    #endregion

    public Patrol(Transform[] wy, Animator anim, Transform Rt  , float sp, float Wd, float St, TypeEnemy typeEnemy)
    {
        Waypoints = wy;
        MyAnimator = anim;
        Root = Rt;
        speed = sp;
        wanderDistance = Wd;
        steeringForce = St;
        this.typeEnemy = typeEnemy;
    }


    public override void OnBegin()
    {

        Debug.Log("Patrol recibió: " + (Waypoints != null ? Waypoints.Length.ToString() : "NULL") + " waypoints");
    
    }

    public override void OnUpdate()
    {
        Move();
    }


    void Move()
    {

        if (Waypoints == null || Waypoints.Length == 0) return;

        targetPos = Waypoints[destPoint].position;
        dir = targetPos - Root.position;

        if (dir.magnitude < wanderDistance)
        {
            destPoint += direction;

            if (destPoint >= Waypoints.Length)
            {
                destPoint = Waypoints.Length - 2;
                direction = -1;
            }
            else if (destPoint < 0)
            {
                destPoint = 1;
                direction = 1;
            }

            targetPos = Waypoints[destPoint].position;
            Root.LookAt(targetPos);
        }
        desired = dir.normalized * speed;

        steering = desired - velocity;

        steering = Vector3.ClampMagnitude(steering, steeringForce);

        velocity = Vector3.ClampMagnitude(velocity + steering, speed);

        Root.position += velocity * Time.deltaTime;
    }



    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    public override void OnEnd()
    {
        
    }

}




