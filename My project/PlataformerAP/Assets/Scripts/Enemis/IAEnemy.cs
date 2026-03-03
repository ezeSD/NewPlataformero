
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public enum EnemyState
{
    Patrol,
    Follow,
    Attack,
    Dead
}

public class IAEnemy : MonoBehaviour
{
     private Transform targetPlayer;
    private Transform Root;
     private Animator Myanimator;
    private FieldOfView View;
    private ConeFieldOfView ConeView;
    [SerializeField] float Speed = 2f;
    [SerializeField] float Stering = 0.1f;

    [Header("Patrol")]
    [SerializeField] Transform[] Waypoints;
    [SerializeField] float patrolWanderDistance = 1f;



    [Header("Attack")]
    [SerializeField] TypeEnemy typeEnemy;
    private EnemyArmor RangeEnemy;
    private FlyEnemyArmor FlyEnemy;


    [Header("Follow")]
    [SerializeField] float  stoppingDistance = 1f;

    [Header("Gizmos")]

    Waypoints waypointScript;
    public FSM<EnemyState> fsm;



    private void Start()
    {
        Myanimator = GetComponent<Animator>();
        Root = GetComponent<Transform>();
        waypointScript = GetComponentInParent<Waypoints>();
        View = GetComponent<FieldOfView>();
        ConeView = GetComponent<ConeFieldOfView>();

        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        getArmor();
        SetWaypoints(waypointScript.GetWaypoints());

        InitializeFSM();
    }


    private void Update()
    {
        if (View != null)
        {
            View.SetFSM(fsm);
        }

        if (ConeView != null)
        {
            ConeView.SetFSM(fsm);
        }


        if (fsm != null)
        {
            fsm.UpdateFSM();
        }

    }


    void InitializeFSM()
    {

        Follow<EnemyState> follow = new Follow<EnemyState>(Root,targetPlayer, Myanimator, Speed,Stering,typeEnemy,stoppingDistance);
        Attack<EnemyState> combat = new Attack<EnemyState>(Root,targetPlayer, Myanimator, typeEnemy,RangeEnemy,FlyEnemy);

        Patrol <EnemyState> patrolState = new Patrol<EnemyState>(Waypoints, Myanimator, Root, Speed, patrolWanderDistance, Stering, typeEnemy);
        Death<EnemyState> Death = new Death<EnemyState>(this.gameObject);

        fsm = new FSM<EnemyState>(patrolState);

        patrolState.AddTransitionsState(new Transition<EnemyState>[]
        {
                new Transition<EnemyState>(EnemyState.Follow, follow),
                new Transition<EnemyState>(EnemyState.Dead, Death)
        });


        follow.AddTransitionsState(new Transition<EnemyState>[]
        {
                new Transition<EnemyState>(EnemyState.Attack, combat),
                new Transition<EnemyState>(EnemyState.Patrol, patrolState),
                new Transition<EnemyState>(EnemyState.Dead, Death),
        });

        combat.AddTransitionsState(new Transition<EnemyState>[]
        {
            new Transition<EnemyState>(EnemyState.Follow, follow),
            new Transition<EnemyState>(EnemyState.Patrol, patrolState),
            new Transition<EnemyState>(EnemyState.Dead, Death)
        });


        patrolState.SetFSM(fsm);
        combat.SetFSM(fsm);
        follow.SetFSM(fsm);
        fsm.StartFSM();

    }

    void getArmor()
    {
        if (typeEnemy == TypeEnemy.Fly)
        {
            FlyEnemy = GetComponent<FlyEnemyArmor>();
        }
        else if (typeEnemy == TypeEnemy.Range)
        {
            RangeEnemy = GetComponent<EnemyArmor>();
        }
        else if (typeEnemy == TypeEnemy.Melee)
        {
            RangeEnemy = null;  
             FlyEnemy = null;
        }
    }

    public void SetWaypoints(Transform[] wp)
    {
        Waypoints = new Transform[wp.Length];
        for (int i = 0; i < wp.Length; i++)
        {
            Waypoints[i] = wp[i];
        }

        Debug.Log("Waypoints asignados: " + Waypoints.Length);

    }
}


