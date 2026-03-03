
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
    Dead
}

public class IAEnemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] int range = 5;
    [SerializeField] int rangeFollow = 2;
    [SerializeField] Transform[] Waipons;

    [SerializeField] Transform targetPlayer;
    [SerializeField] Transform root;


    
    List<Node> pathNodes = new List<Node>();

    [Header("A*")]
    [SerializeField] LayerMask nodeMask;
    [SerializeField] float radio;

    [Header("Gizmos")]
    [SerializeField] bool showFielOfView = true;
    [SerializeField] bool showWander = true;
    [SerializeField] bool showAstar = true;

    private Vector3 lastSeenPosition;
    public FSM<EnemyState> fsm;

    public IcanSeePlayer vision;
    Patrol<EnemyState> patrolState;
    public IAEnemy instance;
    private bool hasReceivedAlert = false;
    Vector3 Post = Vector3.zero;
    private void Start()
    {
        InitializeFSM();
        instance = GetComponent<IAEnemy>();
        vision = GetComponent<IcanSeePlayer>();
        Post = Waipons[0].position;

    }

    private void Update()
    {
        if (fsm != null)
        {
            fsm.UpdateFSM();
        }

    }

    void InitializeFSM()
    {

        Follow<EnemyState> follow = new Follow<EnemyState>(pathNodes, root, nodeMask, radio, targetPlayer, Post);
        Attack<EnemyState> combat = new Attack<EnemyState>();
     
        patrolState = new Patrol<EnemyState>(root, Waipons, range, rangeFollow, targetPlayer);
        Death<EnemyState> Death = new Death<EnemyState>(this.gameObject);


        patrolState.SetFSM(fsm);
        follow.SetFSM(fsm);



        patrolState.AddTransitionsState(new Transition<EnemyState>[]
        {
                new Transition<EnemyState>(EnemyState.InRange, follow),
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
                new Transition<EnemyState>(EnemyState.Dead, Death)
        });
        fsm = new FSM<EnemyState>(patrolState);

        patrolState.SetFSM(fsm);
        vision.SetFSM(fsm);
        vision.setFollow(follow);
        fsm.StartFSM();

    }

    public void inputs(bool bl , EnemyState st)
    {
       
        if (bl)
        {
            fsm.SendInput(st);
        }

      
    }

    public void setTransform(Transform tr)
    {
        targetPlayer = tr;
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


