using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private FSM<EnemyState> _fsm;
    Transform target;
    Transform root;
    LayerMask toDetect;

    [SerializeField] float distMin = 10f;
    [SerializeField] float angleMin = 90f;
    [SerializeField] bool isFOV = false;


    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        root = GetComponent<Transform>();
        toDetect = LayerMask.GetMask("Player");
    }


    void Update()
    {
        ViewPlayer();

        
    }


    void ViewPlayer()
    {
        Vector3 dir = target.position - root.position;
        float dist = Vector3.SqrMagnitude(dir);

        if (dist < distMin * distMin)
        {
            print("Dist");
            Vector3 forward = root.forward;

            float angle = Vector3.Angle(forward, dir);

            if (angle < angleMin * 0.5f)
            {
                print("Angle");
                Ray ray = new Ray(root.position, dir);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit, dist, toDetect))
                {
                    print("Raycast");

                    if ((toDetect & 1 << hit.collider.gameObject.layer) != 0)
                    {
                        print("Lo veo");
                        isFOV = true;
                        if (_fsm != null)
                            _fsm.SendInput(EnemyState.Follow);
                    }
                }
                
            }

        }
        else
        {
            isFOV = false;
            if(_fsm != null)
                _fsm.SendInput(EnemyState.Patrol);
        }
    }


    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }


    private void OnDrawGizmos()
    {
        if (target == null || root == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(root.position, target.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(root.position, distMin);

        Vector3 left = Quaternion.Euler(0,-angleMin /2f, 0) * transform.forward;
        Vector3 rigth = Quaternion.Euler(0, angleMin /2f, 0) * transform.forward;
        Gizmos.color = Color.red;

        Gizmos.DrawRay(root.position, left * distMin);
        Gizmos.DrawRay(root.position, rigth * distMin);

    }
}
