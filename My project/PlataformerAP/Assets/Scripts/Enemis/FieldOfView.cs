using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeFieldOfView : MonoBehaviour
{
    private FSM<EnemyState> _fsm;
    Transform root;
    Transform target;
    LayerMask toDetect;

    [SerializeField] float distMin = 10f;
    [SerializeField] float angleMin = 90f;


    [SerializeField] bool isFOV = false;

    bool wasSeeingPlayer = false;



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

        bool currentlySeeing = false;

        if (dist < distMin * distMin)
        {
            Vector3 down = -root.up;
            float angle = Vector3.Angle(down, dir);

            if (angle < angleMin * 0.5f)
            {
                Ray ray = new Ray(root.position, dir);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Sqrt(dist), toDetect))
                {
                    if ((toDetect & (1 << hit.collider.gameObject.layer)) != 0)
                    {
                        currentlySeeing = true;
                    }
                }
            }
        }

        if (currentlySeeing && !wasSeeingPlayer)
        {
            _fsm.SendInput(EnemyState.Follow);
        }
        else if (!currentlySeeing && wasSeeingPlayer)
        {
            _fsm.SendInput(EnemyState.Patrol);
        }

        wasSeeingPlayer = currentlySeeing;
        isFOV = currentlySeeing;

    }


    public void SetFSM(FSM<EnemyState> fsmInstance)
    {
        _fsm = fsmInstance;
    }

    private void OnDrawGizmos()
    {
        if (root == null)
            return;

        int segments = 4; 
        float radius = Mathf.Tan(angleMin * Mathf.Deg2Rad * 0.5f) * distMin;

        Vector3 origin = root.position;
        Vector3 down = -root.up;

        Vector3 coneEnd = origin + down * distMin;

        Gizmos.color = Color.red;

        Vector3 lastPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * 360f;

            Vector3 dir = Quaternion.AngleAxis(angle, down) * root.forward;
            Vector3 point = coneEnd + dir * radius;


            Gizmos.DrawLine(origin, point);
            if (i > 0)
                Gizmos.DrawLine(lastPoint, point);

            lastPoint = point;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, coneEnd);
    }

}
