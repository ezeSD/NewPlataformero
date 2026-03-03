using IA.PathFinding;
using System.Collections.Generic;
using UnityEngine;

public class IAEnemyFollower : MonoBehaviour
{
    public FSM<EnemyState> fsm;

    [Header("Flocking")]
    [SerializeField] Transform leader;
    [SerializeField] float Speed = 3f;
    [SerializeField] float LeaderRadius = 3f;
    [SerializeField] float LeaderMinRadius = 3f;

    [Header("Thetha*")]
    [SerializeField] LayerMask thetaObstacleMask;
    [SerializeField] Transform Root;

    [Header("Flee")]
    [SerializeField] Transform SafeArea;
    [SerializeField] float detection_radius = 5f;
    [SerializeField] float rango = 5f;
    List<Node> path = new List<Node>();
    [SerializeField] LayerMask lay;


    Heal heal;
    RangeToLeader RangeToLeader;
    


    void Start()
    {
        InitializeFSM();
        if (heal == null)
        {
            heal = GetComponent<Heal>();
        }

        if (RangeToLeader == null)
        {
            RangeToLeader = GetComponent<RangeToLeader>();
        }

        Root = GetComponent<Transform>();
    }

    void Update()
    {
        if (fsm != null)
            fsm.UpdateFSM();

        if(heal != null)
        {
            actualizedHeal();
        }


        if (RangeToLeader != null)
            RangeToLeader.SetFsm(fsm);
    }

    void InitializeFSM()
    {
        AttackFollower<EnemyState> combat = new AttackFollower<EnemyState>();
        DieFollower<EnemyState> death = new DieFollower<EnemyState>();

        PatrolFollower<EnemyState> patrol = new PatrolFollower<EnemyState>(Root, leader, LeaderRadius, LeaderMinRadius);
        LostLeaderFollower<EnemyState> lostLeader = new LostLeaderFollower<EnemyState>(Root, leader, thetaObstacleMask);
        StateFlee<EnemyState> flee = new StateFlee<EnemyState>(SafeArea,path, Root, lay,detection_radius, heal, rango);

        fsm = new FSM<EnemyState>(patrol);

        patrol.AddTransitionsState(new[]{
            new Transition<EnemyState>(EnemyState.LeaderLost, lostLeader),
            new Transition<EnemyState>(EnemyState.flee, flee),
            new Transition<EnemyState>(EnemyState.InRange, combat)
        });

        lostLeader.AddTransitionsState(new[]{
             new Transition<EnemyState>(EnemyState.OutRange, patrol),
             new Transition<EnemyState>(EnemyState.flee, flee),
             new Transition<EnemyState>(EnemyState.Dead, death)
        });

        flee.AddTransitionsState(new[]{
            new Transition<EnemyState>(EnemyState.LeaderLost, lostLeader),
            new Transition<EnemyState>(EnemyState.Dead, death)
        });


        combat.AddTransitionsState(new Transition<EnemyState>[]{
            new Transition<EnemyState>(EnemyState.OutRange, patrol),
            new Transition<EnemyState>(EnemyState.Dead, death)
        });

 
        
        if (fsm != null)
        {
            combat.SetFSM(fsm);
            death.SetFSM(fsm);
            patrol.SetFSM(fsm);
            lostLeader.SetFSM(fsm);
            flee.SetFSM(fsm);

        }
        else
        {
            Debug.LogError("[IAEnemyFollower] fsm sigue siendo NULL justo después de crearlo. Revisa constructor de FSM.");
        }




        FlockManager.instance.AddFollower(transform);

        if (leader == transform)
            FlockManager.instance.SetLeader(transform);

        fsm.StartFSM();

    }

    void actualizedHeal()
    {
        if (heal.healAmount <= heal.minHealth)
        {
            if (fsm != null)
                fsm.SendInput(EnemyState.flee);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rango);
    }


}
