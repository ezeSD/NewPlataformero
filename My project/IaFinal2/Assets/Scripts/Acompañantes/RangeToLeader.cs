using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeToLeader : MonoBehaviour
{

     FSM<EnemyState> _fsm;

    [SerializeField] Transform root;
    [SerializeField] Transform Leader;

    [SerializeField] float distMin = 10f;
    [SerializeField] float angleMin = 90f;

    [SerializeField] float MaxDistToLeader = 20f;


    [SerializeField] bool isFovLeader = false;

    [SerializeField] string leaderTag = "Leader";

    [SerializeField] Heal hp;



    void Update()
    {
        ViewLeader();

    }

    void ViewLeader()
    {
        if (Leader == null) return;

        isFovLeader = false;

        Vector3 dir = Leader.position - root.position;
        float dist = dir.magnitude;

        if (dist > MaxDistToLeader)
        {
            LoseLeader();
            return;
        }

        float angle = Vector3.Angle(root.forward, dir);
        if (angle > angleMin * 0.5f)
        {
            LoseLeader();
            return;
        }

        if (Physics.Raycast(root.position, dir.normalized, out RaycastHit hit, MaxDistToLeader))
        {
            if (hit.collider.CompareTag(leaderTag))
            {
                isFovLeader = true;

                if (_fsm != null)
                {
                    if(hp.healAmount > hp.minHealth)
                    {
                        _fsm.SendInput(EnemyState.OutRange);
                    }
           
                       
                }


                return;
            }
        }

        LoseLeader();
    }

    void LoseLeader()
    {
        isFovLeader = false;
        if(_fsm != null)
        {
            if (hp.healAmount > hp.minHealth)
            {
                _fsm.SendInput(EnemyState.LeaderLost);
            }
        }
            
        
    }


    public void SetFsm(FSM<EnemyState> fsm)
    {
        _fsm = fsm;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, Leader.position);


        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, MaxDistToLeader);

    }



}
