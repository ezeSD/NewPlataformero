using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class IcanSeePlayer : MonoBehaviour
{
    private FSM<EnemyState> _fsm;

    [SerializeField] Transform root;
    [SerializeField] Transform target;

    [SerializeField] float distMin = 10f;
    [SerializeField] float angleMin = 90f;

    [SerializeField] LayerMask toDetect;
    bool isFOV = false;

    Vector3 lastSeenPosition;
    public Follow<EnemyState> FollowState;
    private float timeRemaining = 5f;

    private void Update()
    {
        canseePlayer();
    }


    void canseePlayer()
    {
        Vector3 dir = target.position - root.position;
        float dist = Vector3.SqrMagnitude(dir);
        //Angle();
        if (dist < distMin * distMin)
        {
            Debug.Log("Dist");
            Vector3 forward = root.forward;

            float angle = Vector3.Angle(forward, dir);

            if (angle < angleMin * 0.5f)
            {
                Debug.Log("Angle");
                Ray ray = new Ray(root.position, dir);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit, dist, toDetect))
                {
                    Debug.Log("Raycast");

                    if ((toDetect & 1 << hit.collider.gameObject.layer) != 0)
                    {
                        Transform playerTransform = hit.collider.transform;
                        lastSeenPosition = playerTransform.position; 

                        IAEnemy[] enemies = GameObject.FindObjectsOfType<IAEnemy>();
                        foreach (IAEnemy e in enemies)
                        {
                            e.SetLastSeenPosition(lastSeenPosition); 
                            e.inputs(true , EnemyState.InRange);
                        }
                        Visualiced();
                        _fsm.SendInput(EnemyState.InRange);
                        return;
                    }
                }
            }
           

        }
        _fsm.SendInput(EnemyState.Patrol);
        isFOV = false;
       
    }


    void Visualiced()
    {

        if(transform.position == lastSeenPosition)
        {
            if(isFOV == false)
            {
                IAEnemy enemies = GetComponent<IAEnemy>();
                enemies.SetLastSeenPosition(FollowState.LastWapeon);
                if(transform.position == FollowState.LastWapeon)
                {
                    _fsm.SendInput(EnemyState.Patrol);
                }
            }
        }
    }


    // SISTEMA DE ROTACION NO FUNCIONAL (TODAVIA)
    void Angle()
    {
        timeRemaining = 1f;
        timeRemaining -= Convert.ToInt32(Time.deltaTime);
        int displayTime = Mathf.CeilToInt(timeRemaining);
        Debug.Log(displayTime);

        root.rotation = Quaternion.Euler(0, root.rotation.eulerAngles.y + 90f , 0);

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            Debug.Log("¡Timer terminado!");
            _fsm.SendInput(EnemyState.Patrol);


        }

    }
    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    public void setFollow(Follow<EnemyState> followState)
    {
        FollowState = followState;
    }


    private void OnDrawGizmos()
    {

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(root.position, target.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(root.position, distMin);

            Vector3 left = Quaternion.Euler(0, -angleMin / 2f, 0) * transform.forward;
            Vector3 rigth = Quaternion.Euler(0, angleMin / 2f, 0) * transform.forward;
            Gizmos.color = Color.red;

            Gizmos.DrawRay(root.position, left * distMin);
            Gizmos.DrawRay(root.position, rigth * distMin);

    }



}

