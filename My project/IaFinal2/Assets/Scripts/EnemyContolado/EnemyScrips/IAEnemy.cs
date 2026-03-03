
using IA.PathFinding;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public enum EnemyState
{
    Patrol,
    InRange,
    Desperate,
    OutRange,
    Attack,
    Calm,
    LeaderLost,
    flee,
    Dead
}


public class IAEnemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] int range = 5;
    [SerializeField] int rangeFollow = 2;


    public Transform targetPlayer;
    [SerializeField] Transform root;


    [Header("A*")]
    [SerializeField] LayerMask nodeMask;
    [SerializeField] float radio;
    [SerializeField] float RangeToAttak;

    [Header("Gizmos")]
    [SerializeField] bool showFielOfView = true;
    [SerializeField] bool showWander = true;
    [SerializeField] bool showAstar = true;


    [Header("Attack")]
    [SerializeField] GameObject Bullet;
    [SerializeField] float ShootCooldown;


    public Transform TargetPlayer { get; private set; }
    private Vector3 lastSeenPosition;
    public FSM<EnemyState> fsm;

    List<Node> pathNodes = new List<Node>();
    IcanSeePlayer vision;
    Patrol<EnemyState> patrolState;
    IAEnemy instance;
    private bool hasReceivedAlert = false;
    Vector3 Post = Vector3.zero;
    private void Start()
    {
        InitializeFSM();
        instance = GetComponent<IAEnemy>();
        vision = GetComponent<IcanSeePlayer>();
    }

    private void Update()
    {
        if (fsm != null)
        {
            fsm.UpdateFSM();
        }

        float dist = Vector3.Distance(root.position, targetPlayer.position);
        if (dist > RangeToAttak)
        {
            fsm.SendInput(EnemyState.Attack);
            return;
        }


    }

    void InitializeFSM()
    {
        Follow<EnemyState> follow = new Follow<EnemyState>(pathNodes, root, nodeMask, radio, targetPlayer, Post, RangeToAttak);
        Attack<EnemyState> combat = new Attack<EnemyState>(Bullet, ShootCooldown, root, this);

        patrolState = new Patrol<EnemyState>(root, radio , nodeMask);
        Death<EnemyState> Death = new Death<EnemyState>(this.gameObject);

        fsm = new FSM<EnemyState>(patrolState);



        patrolState.AddTransitionsState(new Transition<EnemyState>[]
        {
                new Transition<EnemyState>(EnemyState.InRange, follow),
                  new Transition<EnemyState>(EnemyState.Attack, combat),
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
                new Transition<EnemyState>(EnemyState.InRange, follow),
                new Transition<EnemyState>(EnemyState.Dead, Death)
        });
        fsm = new FSM<EnemyState>(patrolState);

        patrolState.SetFSM(fsm);
        follow.SetFSM(fsm);
        combat.SetFSM(fsm);


        if (vision != null)
            vision.SetFSM(fsm);
        fsm.StartFSM();

    }

    public void inputs(bool bl , EnemyState st)
    {
       
        if (bl)
        {
            fsm.SendInput(st);
        }

      
    }
    public void setTransform(Transform target)
    {
        TargetPlayer = target;
    }

    public void SetLastSeenPosition(Vector3 pos)
    {
        lastSeenPosition = pos;
    }

    public Vector3 GetLastSeenPosition()
    {
        return lastSeenPosition;
    }





    private void OnDrawGizmos()
    {
        if (showWander)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, range);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeFollow);
        }

       
        if (showAstar)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radio);

        }


    }


}


